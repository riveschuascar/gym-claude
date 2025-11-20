using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Interfaces
{
    public interface IDisciplineServiceClient
    {
        Task<Result<IEnumerable<Discipline>>> GetAllDisciplines();
        Task<Result<Discipline>> GetDisciplineById(int id);
        Task<Result<IEnumerable<Discipline>>> GetDisciplinesByIdsAsync(IEnumerable<int> ids);
    }
}