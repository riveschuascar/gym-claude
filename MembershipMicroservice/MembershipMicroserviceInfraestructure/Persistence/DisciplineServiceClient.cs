using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using MembershipMicroservice.MembershipMicroserviceDomain.Ports;

namespace MembershipMicroservice.MembershipMicroserviceInfrastructure.Persistence
{
    public class DisciplineServiceClient : IDisciplineServiceClient
    {
        public Task<Result<IEnumerable<Discipline>>> GetAllDisciplines()
        {
            return Task.FromResult(Result<IEnumerable<Discipline>>.Success(new List<Discipline>()));
        }

        public Task<Result<Discipline>> GetDisciplineById(int id)
        {
            return Task.FromResult(Result<Discipline>.Success(new Discipline { Id = id, Name = $"Disciplina {id}", IsActive = true }));
        }

        public Task<Result<IEnumerable<Discipline>>> GetDisciplinesByIdsAsync(IEnumerable<int> ids)
        {
            var list = ids.Select(i => new Discipline { Id = i, Name = $"Disciplina {i}", IsActive = true }).ToList();
            return Task.FromResult(Result<IEnumerable<Discipline>>.Success(list));
        }
    }
}