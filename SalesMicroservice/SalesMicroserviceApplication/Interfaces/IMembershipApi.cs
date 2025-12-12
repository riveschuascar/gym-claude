using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceApplication.Interfaces
{
    public interface IMembershipApi
    {
        Task<Result> EnsureExists(int membershipId, CancellationToken cancellationToken = default);
    }
}
