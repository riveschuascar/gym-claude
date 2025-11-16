using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;

namespace DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces
{
    public interface IDisciplineService
    {
        Task<Result<IEnumerable<Discipline>>> GetAll();
        Task<Result<Discipline>> GetById(int id);
        Task<Result<Discipline>> Create(Discipline newDiscipline);
        Task<Result<Discipline>> Update(Discipline disciplineToUpdate);
        Task<Result<bool>> Delete(int id);
    }
}
