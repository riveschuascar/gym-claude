using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Ports;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using MembershipMicroservice.MembershipMicroserviceDomain.Validators;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository repo;

        public MembershipService(IMembershipRepository membershipRepository)
        {
            repo = membershipRepository;
        }

        public async Task<Result<Membership>> GetById(int id)
        {
            var membership = await repo.GetById(id);
            if (membership == null)
            {
                return Result<Membership>.Failure($"No se encontró la membresía con ID {id}.");
            }
            return Result<Membership>.Success(membership.Value!);
        }

        public async Task<Result<IEnumerable<Membership>>> GetAll()
        {
            return await repo.GetAll();
        }

        public async Task<Result<Membership>> Create(Membership newMembership)
        {
            var res = MembershipValidators.Validate(newMembership, isUpdate: false);

            if (!res.IsSuccess)
                return res;

            var created = await repo.Create(newMembership);
            return Result<Membership>.Success(created.Value!);
        }

        public async Task<Result<Membership>> Update(Membership membershipToUpdate)
        {
            var validationResult = MembershipValidators.Validate(membershipToUpdate);
            if (validationResult.IsFailure)
            {
                return Result<Membership>.Failure(validationResult.Error);
            }

            var existingMembership = await repo.GetById(membershipToUpdate.Id);
            if (existingMembership == null)
            {
                return Result<Membership>.Failure($"No se encontró la membresía con ID {membershipToUpdate.Id} para actualizar.");
            }

            var updatedMembership = await repo.Update(membershipToUpdate);

            if (!updatedMembership.IsSuccess)
                return Result<Membership>.Failure(updatedMembership.Error);

            return Result<Membership>.Success(updatedMembership.Value!);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            var res = await repo.DeleteById(id);
            if (!res.IsSuccess)
            {
                return Result<bool>.Failure($"No se pudo eliminar la membresía con ID {id}.");
            }
            return Result<bool>.Success(true);
        }
    }
}
