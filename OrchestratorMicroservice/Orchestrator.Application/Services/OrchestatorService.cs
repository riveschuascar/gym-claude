using System.Net.Http.Json;
using Orchestrator.Application.Interfaces;
using Orchestrator.Application.Models;
using Microsoft.Extensions.Logging;

namespace Orchestrator.Application.Services;

public class OrchestratorService : IOrchestatorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrchestratorService> _logger;

    public OrchestratorService(IHttpClientFactory httpClientFactory, ILogger<OrchestratorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task StartSagaAsync(SaleCreatedEvent @event)
    {
        _logger.LogInformation("Starting saga for sale {SaleId}", @event.SaleId);

        // 1) Validate client by GET /api/Clients/{id}
        var clientValid = await ValidateClientAsync(@event.ClientId);
        if (!clientValid)
        {
            _logger.LogWarning("Client {ClientId} not valid", @event.ClientId);
            await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.ClientNotFound);
            return;
        }

        // 2) Insert sale_details via POST /api/SaleDetails
        var detailsCreated = await CreateSaleDetailsAsync(@event.SaleId, @event.Details);
        if (!detailsCreated)
        {
            _logger.LogWarning("Failed to create sale details for sale {SaleId}", @event.SaleId);
            await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.SaleDetailsFailed);
            return;
        }

        // 3) Validate each discipline via PUT /api/Disciplines/validate/ (verifica existencia y actualiza cupos)
        var processedDisciplines = new List<SaleDetailDto>();

        foreach (var d in @event.Details)
        {
            var valid = await ValidateDisciplineAsync(d.DisciplineId, d.Qty);
            if (!valid)
            {
                _logger.LogWarning("Discipline {DisciplineId} validation failed. Starting compensation.", d.DisciplineId);

                // Compensar las disciplinas que ya fueron procesadas
                await CompensateDisciplinesAsync(processedDisciplines);

                await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.DisciplineFailed);
                return;
            }

            // Agregar a la lista de procesadas exitosamente
            processedDisciplines.Add(d);
            _logger.LogInformation("Discipline {DisciplineId} validated and updated successfully", d.DisciplineId);
        }

        // 4) Mark sale as complete
        await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.Completed);

        // 5) Notify reports
        await NotifyReportsAsync(@event.SaleId);
    }

    private async Task<bool> CreateSaleDetailsAsync(int saleId, List<SaleDetailDto> details)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("saleDetails");
            var response = await client.PostAsJsonAsync("/api/SaleDetails", details);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Sale details created successfully for sale {SaleId}", saleId);
                return true;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Bad request when creating sale details for sale {SaleId}: {Error}",
                    saleId, errorContent);
                return false;
            }

            if ((int)response.StatusCode >= 500)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Server error when creating sale details for sale {SaleId}: {StatusCode} - {Error}",
                    saleId, response.StatusCode, errorContent);
                return false;
            }

            _logger.LogWarning("Unexpected status code {StatusCode} when creating sale details for sale {SaleId}",
                response.StatusCode, saleId);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request exception when creating sale details for sale {SaleId}", saleId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when creating sale details for sale {SaleId}", saleId);
            return false;
        }
    }

    private async Task<bool> ValidateClientAsync(int clientId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("clients");
            var resp = await client.GetAsync($"/api/Client/{clientId}");
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            _logger.LogWarning("Unexpected response from clients service: {Status}", resp.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating client {ClientId}", clientId);
            return false;
        }
    }

    private async Task<bool> ValidateDisciplineAsync(int disciplineId, int qty)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("disciplines");

            var resp = await client.PutAsJsonAsync($"/api/Disciplines/validate/{disciplineId}", qty);

            if (resp.IsSuccessStatusCode)
            {
                return true;
            }

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await resp.Content.ReadAsStringAsync();
                _logger.LogWarning("Discipline {DisciplineId} validation failed: {Error}", disciplineId, errorContent);
                return false;
            }

            _logger.LogWarning("Discipline service returned status {Status} for {Id}", resp.StatusCode, disciplineId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating discipline {DisciplineId}", disciplineId);
            return false;
        }
    }

    private async Task CompensateDisciplinesAsync(List<SaleDetailDto> processedDisciplines)
    {
        if (!processedDisciplines.Any())
        {
            _logger.LogInformation("No disciplines to compensate");
            return;
        }

        _logger.LogInformation("Starting compensation for {Count} disciplines", processedDisciplines.Count);

        foreach (var d in processedDisciplines)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("disciplines");
                // Revertir la operación: devolver los cupos sumando qty de nuevo
                var payload = new { Qty = d.Qty };
                var resp = await client.PutAsJsonAsync($"/api/Disciplines/compensate/{d.DisciplineId}", payload);

                if (resp.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Discipline {DisciplineId} compensated successfully (restored {Qty} cupos)",
                        d.DisciplineId, d.Qty);
                }
                else
                {
                    _logger.LogError("Failed to compensate discipline {DisciplineId} with status {Status}",
                        d.DisciplineId, resp.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating discipline {DisciplineId}", d.DisciplineId);
            }
        }
    }

    private async Task UpdateSaleStatusAsync(int saleId, string status)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("sales");
            var payload = status;
            await client.PostAsJsonAsync($"/api/Sales/status/{saleId}", payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sale {SaleId} status to {Status}", saleId, status);
        }
    }

    private async Task NotifyReportsAsync(int saleId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("reports");
            var payload = new { saleId };
            await client.PostAsJsonAsync("/api/Report/sales", payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying reports for sale {SaleId}", saleId);
        }
    }
}
