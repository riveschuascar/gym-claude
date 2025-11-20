using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Ports;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using MembershipMicroservice.MembershipMicroserviceDomain.Validators;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _repo;
        private readonly IDisciplineServiceClient _disciplineClient;

        public MembershipService(
            IMembershipRepository membershipRepository,
            IDisciplineServiceClient disciplineServiceClient)
        {
            _repo = membershipRepository;
            _disciplineClient = disciplineServiceClient;
        }

        public async Task<Result<Membership>> GetById(int id)
        {
            if (id <= 0)
                return Result<Membership>.Failure("El Id debe ser mayor a cero.");

            var membershipResult = await _repo.GetById(id);
            if (!membershipResult.IsSuccess || membershipResult.Value == null)
                return Result<Membership>.Failure($"No se encontró la membresía con ID {id}.");

            var membership = membershipResult.Value;
            return Result<Membership>.Success(membership);
        }

        public async Task<Result<IEnumerable<Membership>>> GetAll()
        {
            var result = await _repo.GetAll();
            if (!result.IsSuccess)
                return Result<IEnumerable<Membership>>.Failure(result.Error!);

            var memberships = result.Value ?? new List<Membership>();
            return Result<IEnumerable<Membership>>.Success(memberships);
        }

        public async Task<Result<Membership>> Create(Membership newMembership)
        {
            var validation = MembershipValidators.Create(newMembership);
            if (validation.IsFailure)
                return Result<Membership>.Failure(validation.Error!);

            var created = await _repo.Create(newMembership);
            if (!created.IsSuccess)
                return Result<Membership>.Failure(created.Error!);

            return Result<Membership>.Success(created.Value!);
        }

        public async Task<Result<Membership>> Update(Membership membershipToUpdate)
        {
            if (membershipToUpdate == null)
                return Result<Membership>.Failure("La membresía no puede ser nula.");

            if (!membershipToUpdate.Id.HasValue || membershipToUpdate.Id.Value <= 0)
                return Result<Membership>.Failure("El Id de la membresía debe ser mayor a cero.");

            int id = membershipToUpdate.Id.Value; // <-- conversión segura

            // Validación
            var validation = MembershipValidators.Update(membershipToUpdate);
            if (validation.IsFailure)
                return Result<Membership>.Failure(validation.Error!);

            var existingMembership = await _repo.GetById(id);
            if (existingMembership == null)
                return Result<Membership>.Failure($"No se encontró la membresía con ID {id} para actualizar.");

            var updated = await _repo.Update(membershipToUpdate);
            if (!updated.IsSuccess)
                return Result<Membership>.Failure(updated.Error!);

            return Result<Membership>.Success(updated.Value!);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            if (id <= 0)
                return Result<bool>.Failure("El Id debe ser mayor a cero.");

            var res = await _repo.DeleteById(id);
            if (!res.IsSuccess)
                return Result<bool>.Failure($"No se pudo eliminar la membresía con ID {id}.");

            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<Discipline>>> GetDisciplinesForMembership(IEnumerable<int> disciplineIds)
        {
            if (disciplineIds == null || !disciplineIds.Any())
                return Result<IEnumerable<Discipline>>.Failure("No se proporcionaron IDs de disciplinas.");

            var res = await _disciplineClient.GetDisciplinesByIdsAsync(disciplineIds);
            return res;
        }
    }
}