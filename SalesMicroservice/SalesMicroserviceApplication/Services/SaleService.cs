using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Events;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using SalesMicroserviceDomain.Ports;
using SalesMicroserviceDomain.Shared;
using SalesMicroserviceDomain.Validators;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;

namespace SalesMicroserviceApplication.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _repo;
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<SaleService> _logger;

        public SaleService(
            ISaleRepository repo,
            IHttpClientFactory httpFactory,
            ILogger<SaleService> logger)
        {
            _repo = repo;
            _httpFactory = httpFactory;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Sale>>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Result<Sale>> GetById(int id)
        {
            return await _repo.GetById(id);
        }

        public async Task<Result<Sale>> Create(Sale sale, string? userEmail = null, OperationContext? context = null)
        {
            context ??= new OperationContext();

            var validation = SaleValidators.ValidateForCreate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            if (sale.Details == null || !sale.Details.Any())
                return Result<Sale>.Failure("La venta debe contener al menos una disciplina.");

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            sale.CreatedAt = DateTime.UtcNow;
            sale.LastModification = DateTime.UtcNow;
            sale.IsActive = true;

            // Calcular el total basado en los detalles recibidos
            foreach (var d in sale.Details)
            {
                if (d.Total <= 0)
                    d.Total = d.Price * d.Qty;
            }
            var total = sale.Details.Sum(d => d.Total);
            if (sale.TotalAmount <= 0) 
                sale.TotalAmount = total;

            var createResult = await _repo.Create(sale, userEmail);
            if (createResult.IsFailure) 
                return createResult;

            // Notificar al orquestador con la lista de disciplinas (best-effort)
            _ = NotifyOrchestratorAsync(createResult.Value);

            return createResult;
        }

        private async Task NotifyOrchestratorAsync(Sale sale)
        {
            try
            {
                var client = _httpFactory.CreateClient("Orchestrator");
                var payload = new
                {
                    idOrquestas = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    saleId = sale.Id ?? 0,
                    clientId = sale.ClientId,
                    disciplinesIds = sale.Details?.Select(d => d.DisciplineId).ToArray() ?? Array.Empty<int>()
                };

                var resp = await client.PostAsJsonAsync("/api/Orchestator/sale-created", payload);

                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Orchestrator notification failed with status {Status} for sale {SaleId}", resp.StatusCode, sale.Id);
                }
                else
                {
                    _logger.LogInformation("Orchestrator notified for sale {SaleId}", sale.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to notify orchestrator for sale {SaleId}", sale.Id);
            }
        }

        public async Task<Result<Sale>> Update(Sale sale, string? userEmail = null)
        {
            var validation = SaleValidators.ValidateForUpdate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            sale.LastModification = DateTime.UtcNow;

            return await _repo.Update(sale, userEmail);
        }

        public async Task<Result> Delete(int id, string? userEmail = null)
        {
            return await _repo.Delete(id, userEmail);
        }

        public async Task<Result> UpdateSaleStatus(int Id, string Status)
        {
            _logger.LogInformation("Updating sale {SaleId} status to {Status}", Id, Status);
            return await _repo.UpdateSaleStatus(Id, Status);
        }
    }
}