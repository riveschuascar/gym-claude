using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Events;
using SalesMicroserviceDomain.Ports;
using SalesMicroserviceDomain.Shared;
using SalesMicroserviceDomain.Validators;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SalesMicroserviceApplication.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _repo;
        private readonly IClientApi _clientApi;
        private readonly IMembershipApi _membershipApi;
        private readonly IOutboxRepository _outbox;
        private readonly ILogger<SaleService> _logger;

        public SaleService(
            ISaleRepository repo,
            IClientApi clientApi,
            IMembershipApi membershipApi,
            IOutboxRepository outbox,
            ILogger<SaleService> logger)
        {
            _repo = repo;
            _clientApi = clientApi;
            _membershipApi = membershipApi;
            _outbox = outbox;
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

            var clientExists = await _clientApi.EnsureExists(sale.ClientId);
            if (clientExists.IsFailure)
                return Result<Sale>.Failure(clientExists.Error!);

            var membershipExists = await _membershipApi.EnsureExists(sale.MembershipId);
            if (membershipExists.IsFailure)
                return Result<Sale>.Failure(membershipExists.Error!);

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            sale.StartDate = sale.SaleDate;
            sale.EndDate = sale.SaleDate;
            sale.CreatedAt = DateTime.UtcNow;
            sale.LastModification = DateTime.UtcNow;
            sale.IsActive = true;

            var createResult = await _repo.Create(sale, userEmail);
            if (createResult.IsFailure) return createResult;

            var saleCreatedEvent = SaleCreatedEvent.FromSale(createResult.Value!, context.CorrelationId, context.OperationId);
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = nameof(SaleCreatedEvent),
                Payload = JsonSerializer.Serialize(saleCreatedEvent),
                OccurredOn = DateTime.UtcNow,
                CorrelationId = saleCreatedEvent.CorrelationId,
                OperationId = saleCreatedEvent.OperationId
            };

            var outboxResult = await _outbox.SaveAsync(outboxMessage);
            if (outboxResult.IsFailure)
            {
                _logger.LogError("No se pudo guardar en outbox el evento de venta creada. Error: {Error}", outboxResult.Error);
            }

            return createResult;
        }

        public async Task<Result<Sale>> Update(Sale sale, string? userEmail = null)
        {
            var validation = SaleValidators.ValidateForUpdate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            var clientExists = await _clientApi.EnsureExists(sale.ClientId);
            if (clientExists.IsFailure)
                return Result<Sale>.Failure(clientExists.Error!);

            var membershipExists = await _membershipApi.EnsureExists(sale.MembershipId);
            if (membershipExists.IsFailure)
                return Result<Sale>.Failure(membershipExists.Error!);

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            sale.StartDate = sale.SaleDate;
            sale.EndDate = sale.SaleDate;
            sale.LastModification = DateTime.UtcNow;
            return await _repo.Update(sale, userEmail);
        }

        public async Task<Result> Delete(int id, string? userEmail = null)
        {
            return await _repo.Delete(id, userEmail);
        }
    }
}
