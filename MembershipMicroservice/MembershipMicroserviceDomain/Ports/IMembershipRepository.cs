using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Ports
{
    public interface IMembershipRepository
    {
        Task<Result<Membership>> GetById(int id);
        Task<Result<IEnumerable<Membership>>> GetAll();
        Task<Result<Membership>> Create(Membership entity);
        Task<Result<Membership>> Update(Membership entity);
        Task<Result> DeleteById(int id);
    }
}