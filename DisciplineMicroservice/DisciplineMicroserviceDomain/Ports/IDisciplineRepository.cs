using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisciplineMicroservice.DisciplineMicroserviceDomain.Ports
{
    public interface IDisciplineRepository
    {
        Task<Result<IEnumerable<Discipline>>> GetAll();
        Task<Result<Discipline>> GetById(int id);
        Task<Result<Discipline>> Create(Discipline entity, string? userEmail = null);
        Task<Result<Discipline>> Update(Discipline entity, string? userEmail = null);
        Task<Result> DeleteById(int id, string? userEmail = null);
        Task<Result> UpdateCupos(short id, int qty, string? userEmail);
    }
}