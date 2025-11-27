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

        public MembershipService(IMembershipRepository membershipRepository)
        {
            _repo = membershipRepository;
        }

        public async Task<Result<Membership>> GetById(int id)
        {
            var membership = await _repo.GetById(id);
            if (membership == null)
                return Result<Membership>.Failure($"No se encontró la membresía con ID {id}.");

            return Result<Membership>.Success(membership.Value!);
        }

        public async Task<Result<IEnumerable<Membership>>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Result<Membership>> Create(Membership newMembership, string? userEmail = null)
        {
            // Validación
            var validation = MembershipValidators.Create(newMembership);
            if (validation.IsFailure)
                return Result<Membership>.Failure(validation.Error!);

            var created = await _repo.Create(newMembership, userEmail);
            if (!created.IsSuccess)
                return Result<Membership>.Failure(created.Error!);

            return Result<Membership>.Success(created.Value!);
        }

        public async Task<Result<Membership>> Update(Membership membershipToUpdate, string? userEmail = null)
        {
            // Validación
            var validation = MembershipValidators.Update(membershipToUpdate);
            if (validation.IsFailure)
                return Result<Membership>.Failure(validation.Error!);

            // Desempaquetamos nullable Id
            var id = membershipToUpdate.Id ?? 0;
            if (id <= 0)
                return Result<Membership>.Failure("El Id de la membresía debe ser mayor a cero.");

            var existingMembership = await _repo.GetById(id);
            if (existingMembership == null)
                return Result<Membership>.Failure($"No se encontró la membresía con ID {id} para actualizar.");

            var updated = await _repo.Update(membershipToUpdate, userEmail);
            if (!updated.IsSuccess)
                return Result<Membership>.Failure(updated.Error!);

            return Result<Membership>.Success(updated.Value!);
        }

        public async Task<Result<bool>> Delete(int id, string? userEmail = null)
        {
            var res = await _repo.DeleteById(id, userEmail);
            if (!res.IsSuccess)
                return Result<bool>.Failure($"No se pudo eliminar la membresía con ID {id}.");

            return Result<bool>.Success(true);
        }
    }
}
