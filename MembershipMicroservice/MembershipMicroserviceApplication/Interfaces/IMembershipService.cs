using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Interfaces
{
    public interface IMembershipService
    {
        Task<Result<IEnumerable<Membership>>> GetAll();
        Task<Result<Membership>> GetById(int id);
        Task<Result<Membership>> Create(Membership newMembership, string? userEmail = null);
        Task<Result<Membership>> Update(Membership membershipToUpdate, string? userEmail = null);
        Task<Result<bool>> Delete(int id, string? userEmail = null);
    }
}
