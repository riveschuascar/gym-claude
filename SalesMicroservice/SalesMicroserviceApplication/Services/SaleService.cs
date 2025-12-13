using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Events;
using System.Net;
using System.Net.Http;
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

        public async Task<Result<IEnumerable<Sale>>> GetAll() => await _repo.GetAll();

        public async Task<Result<Sale>> GetById(int id) => await _repo.GetById(id);

        public async Task<Result<Sale>> Create(Sale sale, string? userEmail = null, OperationContext? context = null)
        {
            context ??= new OperationContext();

            var validation = SaleValidators.ValidateForCreate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            var clientExists = await EnsureClientExists(sale.ClientId);
            if (clientExists.IsFailure)
                return Result<Sale>.Failure(clientExists.Error!);

            // validate each discipline in the details
            if (sale.Details != null)
            {
                foreach (var d in sale.Details)
                {
                    var disciplineExists = await EnsureDisciplineExists(d.DisciplineId);
                    if (disciplineExists.IsFailure)
                        return Result<Sale>.Failure(disciplineExists.Error!);
                }
            }

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            // details contain start/end per discipline, if needed leave them as-is
            sale.CreatedAt = DateTime.UtcNow;
            sale.LastModification = DateTime.UtcNow;
            sale.IsActive = true;
            // compute totals for details and sale total
            if (sale.Details != null)
            {
                foreach (var d in sale.Details)
                {
                    if (d.Total <= 0)
                        d.Total = d.Price * d.Qty;
                }
                var total = sale.Details.Sum(d => d.Total);
                if (sale.TotalAmount <= 0) sale.TotalAmount = total;
            }

            var createResult = await _repo.Create(sale, userEmail);
            if (createResult.IsFailure) return createResult;

            // outbox and event publishing omitted for now - handled later

            return createResult;
        }

        public async Task<Result<Sale>> Update(Sale sale, string? userEmail = null)
        {
            var validation = SaleValidators.ValidateForUpdate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            var clientExists = await EnsureClientExists(sale.ClientId);
            if (clientExists.IsFailure)
                return Result<Sale>.Failure(clientExists.Error!);

            if (sale.Details != null)
            {
                foreach (var d in sale.Details)
                {
                    var disciplineExists = await EnsureDisciplineExists(d.DisciplineId);
                    if (disciplineExists.IsFailure)
                        return Result<Sale>.Failure(disciplineExists.Error!);
                }
            }

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            // details contained by sale, no global start/end
            sale.LastModification = DateTime.UtcNow;
            if (sale.Details != null)
            {
                foreach (var d in sale.Details)
                {
                    if (d.Total <= 0)
                        d.Total = d.Price * d.Qty;
                }
                var total = sale.Details.Sum(d => d.Total);
                if (sale.TotalAmount <= 0) sale.TotalAmount = total;
            }
            return await _repo.Update(sale, userEmail);
        }

        public async Task<Result> Delete(int id, string? userEmail = null)
        {
            return await _repo.Delete(id, userEmail);
        }

        private async Task<Result> EnsureClientExists(int clientId)
        {
            try
            {
                var client = _httpFactory.CreateClient("Clients");
                var response = await client.GetAsync($"/api/Client/{clientId}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Result.Failure("El cliente no existe en el microservicio de clientes.");

                response.EnsureSuccessStatusCode();
                return Result.Success();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Timeout al validar cliente {ClientId}. Se permite continuar de forma optimista.", clientId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo validar cliente {ClientId} en Client API. Se continua optimistamente.", clientId);
                return Result.Success();
            }
        }

        private async Task<Result> EnsureDisciplineExists(int disciplineId)
        {
            try
            {
                var client = _httpFactory.CreateClient("Disciplines");
                var response = await client.GetAsync($"/api/Disciplines/{disciplineId}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Result.Failure("La disciplina no existe en el microservicio de disciplinas.");

                response.EnsureSuccessStatusCode();
                return Result.Success();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Timeout al validar disciplina {DisciplineId}. Se permite continuar de forma optimista.", disciplineId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo validar disciplina {DisciplineId} en Discipline API. Se continua optimistamente.", disciplineId);
                return Result.Success();
            }
        }
    }
}
