using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;

namespace DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces
{
    public interface IDisciplineService
    {
        Task<Result<IEnumerable<Discipline>>> GetAll();
        Task<Result<Discipline>> GetById(int id);
        Task<Result<Discipline>> Create(Discipline newDiscipline, string? userEmail = null);
        Task<Result<Discipline>> Update(Discipline disciplineToUpdate, string? userEmail = null);
        Task<Result<bool>> Delete(int id, string? userEmail = null);
    }
}