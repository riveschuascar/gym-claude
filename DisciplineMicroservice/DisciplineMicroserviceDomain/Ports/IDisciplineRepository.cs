using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;
using System.Threading.Tasks;

namespace DisciplineMicroservice.DisciplineMicroserviceDomain.Ports
{
    public interface IDisciplineRepository
    {
        Task<Result<IEnumerable<Discipline>>> GetAll();
        Task<Result<Discipline>> GetById(int id);
        Task<Result<Discipline>> Create(Discipline entity);
        Task<Result<Discipline>> Update(Discipline entity);
        Task<Result> DeleteById(int id);
        Task<Result<IEnumerable<Discipline>>> GetAll(int id);
    }
}
