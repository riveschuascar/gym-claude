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

        // 1) Validate client by GET /api/clients/{id}
        var clientValid = await ValidateClientAsync(@event.ClientId);
        if (!clientValid)
        {
            _logger.LogWarning("Client {ClientId} not valid", @event.ClientId);
            await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.ClientNotFound);
            return;
        }

        // 2) Validate each discipline via POST /api/disciplines/validate
        foreach (var d in @event.DisciplinesIds)
        {
            var valid = await ValidateDisciplineAsync(d);
            if (!valid)
            {
                _logger.LogWarning("Discipline {DisciplineId} not valid", d);
                await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.DisciplineFailed);
                return;
            }
        }

        // 3) Mark sale as complete
        await UpdateSaleStatusAsync(@event.SaleId, SaleStatus.Completed);

        // 4) Notify reports
        await NotifyReportsAsync(@event.SaleId);  
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

    private async Task<bool> ValidateDisciplineAsync(int disciplineId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("disciplines");
            var payload = new { id = disciplineId };
            var resp = await client.GetAsync($"/api/Disciplines/{disciplineId}");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Discipline service returned status {Status} for {Id}", resp.StatusCode, disciplineId);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating discipline {DisciplineId}", disciplineId);
            return false;
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
