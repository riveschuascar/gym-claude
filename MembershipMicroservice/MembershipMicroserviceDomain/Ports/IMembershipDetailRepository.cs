using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Ports
{
    public interface IMembershipDetailRepository
    {
        Task<Result<IEnumerable<MembershipDiscipline>>> GetByMembershipId(int membershipId);
        Task<Result<MembershipDiscipline>> Create(MembershipDiscipline entity, string? userEmail = null);
        Task<Result> Delete(int membershipId, int disciplineId, string? userEmail = null);
    }
}
