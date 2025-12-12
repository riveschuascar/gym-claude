using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceApplication.Interfaces
{
    public interface IClientApi
    {
        Task<Result> EnsureExists(int clientId, CancellationToken cancellationToken = default);
    }
}
