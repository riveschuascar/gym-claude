using DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces;
using DisciplineMicroservice.DisciplineMicroserviceApplication.Services;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Ports;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Validators;

namespace DisciplineMicroservice.DisciplineMicroserviceApplication.Services
{
    public class DisciplineService : IDisciplineService
    {
        private readonly IDisciplineRepository repo;

        public DisciplineService(IDisciplineRepository disciplineRepository)
        {
            repo = disciplineRepository;
        }

        public async Task<Result<Discipline>> Create(Discipline newDiscipline, string? userEmail = null)
        {
            var res = DisciplineValidators.Validate(newDiscipline);

            if (!res.IsSuccess)
            {
                return res;
            }

            res = await repo.Create(newDiscipline, userEmail);
            if (!res.IsSuccess)
            {
                return Result<Discipline>.Failure(res.Error ?? "No se pudo crear la disciplina.");
            }

            return Result<Discipline>.Success(res.Value!);
        }

        public async Task<Result<Discipline>> Update(Discipline disciplineToUpdate, string? userEmail = null)
        {
            var validationResult = DisciplineValidators.Validate(disciplineToUpdate);
            if (validationResult.IsFailure)
            {
                return Result<Discipline>.Failure(validationResult.Error);
            }

            var existingDiscipline = await repo.GetById(disciplineToUpdate.Id);
            if (!existingDiscipline.IsSuccess || existingDiscipline.Value is null)
            {
                return Result<Discipline>.Failure($"No se encontró la disciplina con ID {disciplineToUpdate.Id} para actualizar.");
            }

            var updatedDiscipline = await repo.Update(disciplineToUpdate, userEmail);

            if (!updatedDiscipline.IsSuccess || updatedDiscipline.Value is null)
            {
                return Result<Discipline>.Failure(updatedDiscipline.Error ?? "No se pudo actualizar la disciplina.");
            }

            return Result<Discipline>.Success(updatedDiscipline.Value);
        }

        public async Task<Result<Discipline>> GetById(int id)
        {
            var discipline = await repo.GetById(id);
            if (!discipline.IsSuccess || discipline.Value is null)
            {
                return Result<Discipline>.Failure($"No se encontró la disciplina con ID {id}.");
            }
            return Result<Discipline>.Success(discipline.Value!);
        }

        public async Task<Result<bool>> Delete(int id, string? userEmail = null)
        {
            var res = await repo.DeleteById(id, userEmail);
            if (!res.IsSuccess)
            {
                return Result<bool>.Failure($"No se pudo eliminar la disciplina con ID {id}.");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<Discipline>>> GetAll()
        {
            return await repo.GetAll();
        }

        public async Task<Result> Validate(short id, int qty, string? email)
        {
            var discipline = await repo.GetById(id);
            if (!discipline.IsSuccess || discipline.Value is null)
            {
                return Result.Failure($"No se encontro la disciplina con Id {id}");
            }
            int newQty = (int)(discipline.Value.Cupos - qty)!;
            if (newQty < 0)
            {
                return Result.Failure("No existen suficientes cupos");
            }    
            return await repo.UpdateCupos(id, newQty, email);
        }
    }
}