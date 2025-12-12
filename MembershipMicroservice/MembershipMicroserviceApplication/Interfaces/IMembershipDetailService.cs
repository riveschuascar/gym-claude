using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Interfaces
{
    public interface IMembershipDetailService
    {
        Task<Result<IEnumerable<MembershipDiscipline>>> GetByMembership(int membershipId);
        Task<Result<MembershipDiscipline>> AddDiscipline(int membershipId, int disciplineId, string? userEmail = null);
        Task<Result<bool>> RemoveDiscipline(int membershipId, int disciplineId, string? userEmail = null);
    }
}
