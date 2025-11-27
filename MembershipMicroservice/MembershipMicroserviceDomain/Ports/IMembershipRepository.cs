using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Ports
{
    public interface IMembershipRepository
    {
        Task<Result<IEnumerable<Membership>>> GetAll();
        Task<Result<Membership>> GetById(int id);
        Task<Result<Membership>> Create(Membership entity, string? userEmail = null);
        Task<Result<Membership>> Update(Membership entity, string? userEmail = null);
        Task<Result> DeleteById(int id, string? userEmail = null);
    }
}