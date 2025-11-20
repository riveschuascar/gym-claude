using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Interfaces
{
    public interface IMembershipService
    {
        Task<Result<Membership>> GetById(int id);
        Task<Result<IEnumerable<Membership>>> GetAll();
        Task<Result<Membership>> Create(Membership newMembership);
        Task<Result<Membership>> Update(Membership membershipToUpdate);
        Task<Result<bool>> Delete(int id);
    }
}
