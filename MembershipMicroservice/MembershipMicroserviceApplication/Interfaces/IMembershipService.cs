using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Interfaces
{
    public class IMembershipService
    {
        Task<Result<Membership>> GetById(int id);
        Task<Result<IReadOnlyCollection<Membership>>> GetAll();
        Task<Result<Membership>> Create(Membership newMembership);
        Task<Result<Membership>> Update(Membership membershipToUpdate);
        Task<Result> Delete(int id);
    }
}
