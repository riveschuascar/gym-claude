using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceDomain.Ports
{
    public interface IOutboxRepository
    {
        Task<Result> SaveAsync(OutboxMessage message);
    }
}
