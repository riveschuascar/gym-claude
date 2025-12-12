using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Ports;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using MembershipMicroservice.MembershipMicroserviceDomain.Validators;

namespace MembershipMicroservice.MembershipMicroserviceApplication.Services
{
    public class MembershipDetailService : IMembershipDetailService
    {
        private readonly IMembershipDetailRepository _detailRepository;
        private readonly IMembershipRepository _membershipRepository;

        public MembershipDetailService(
            IMembershipDetailRepository detailRepository,
            IMembershipRepository membershipRepository)
        {
            _detailRepository = detailRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<Result<IEnumerable<MembershipDiscipline>>> GetByMembership(int membershipId)
        {
            if (membershipId <= 0)
                return Result<IEnumerable<MembershipDiscipline>>.Failure("El Id de la membresia es obligatorio.");

            return await _detailRepository.GetByMembershipId(membershipId);
        }

        public async Task<Result<MembershipDiscipline>> AddDiscipline(int membershipId, int disciplineId, string? userEmail = null)
        {
            var detail = new MembershipDiscipline
            {
                MembershipId = membershipId,
                DisciplineId = disciplineId
            };

            var validation = MembershipDetailValidators.Validate(detail);
            if (validation.IsFailure)
                return Result<MembershipDiscipline>.Failure(validation.Error!);

            var membershipExists = await _membershipRepository.GetById(membershipId);
            if (membershipExists == null || membershipExists.IsFailure || membershipExists.Value == null)
                return Result<MembershipDiscipline>.Failure($"No se encontro la membresia con Id {membershipId}.");

            return await _detailRepository.Create(detail, userEmail);
        }

        public async Task<Result<bool>> RemoveDiscipline(int membershipId, int disciplineId, string? userEmail = null)
        {
            if (membershipId <= 0 || disciplineId <= 0)
                return Result<bool>.Failure("Ids de membresia y disciplina son obligatorios.");

            var deleted = await _detailRepository.Delete(membershipId, disciplineId, userEmail);
            return deleted.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(deleted.Error ?? "No se pudo eliminar la disciplina de la membresia.");
        }
    }
}
