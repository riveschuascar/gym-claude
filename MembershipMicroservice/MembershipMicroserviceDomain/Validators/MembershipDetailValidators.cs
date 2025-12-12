using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Validators
{
    public static class MembershipDetailValidators
    {
        public static Result<MembershipDiscipline> Validate(MembershipDiscipline detail)
        {
            if (detail == null)
                return Result<MembershipDiscipline>.Failure("El detalle de membresia no puede ser nulo.");

            if (detail.MembershipId <= 0)
                return Result<MembershipDiscipline>.Failure("El Id de la membresia es obligatorio.");

            if (detail.DisciplineId <= 0)
                return Result<MembershipDiscipline>.Failure("El Id de la disciplina es obligatorio.");

            return Result<MembershipDiscipline>.Success(detail);
        }
    }
}
