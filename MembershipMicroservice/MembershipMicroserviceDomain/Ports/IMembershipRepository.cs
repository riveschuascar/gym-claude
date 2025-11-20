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
        Task<Result<bool>> UpdateMembershipDisciplines(int membershipId, IEnumerable<int> disciplineIds);
        Task<Result<IEnumerable<int>>> GetMembershipDisciplineIds(int membershipId);
    }
}