using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using System.Text.RegularExpressions;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Validators
{
    public static class MembershipValidators
    {
        private static readonly Regex AllowedCharsRegex =
            new Regex("^[a-zA-Z0-9 ñáéíóúÁÉÍÓÚüÜ]+$", RegexOptions.Compiled);

        public static Result<Membership> Validate(Membership membership, bool isUpdate)
        {
            if (membership is null)
            {
                return Result<Membership>.Failure("La disciplina no puede quedar vacía.");
            }

            if (membership.Id <= 0)
            {
                return Result<Membership>.Failure("El identificador de la membresía debe ser mayor a cero.");
            }

            if (string.IsNullOrWhiteSpace(membership.Name))
                return Result<Membership>.Failure("El nombre de la disciplina es obligatorio.");

            else if (membership.Name.Length > 20)
            {
                return Result<Membership>.Failure("El nombre no puede exceder los 50 caracteres.");
            }

            if (!AllowedCharsRegex.IsMatch(membership.Name))
                return Result<Membership>.Failure("El nombre contiene caracteres no permitidos.");

            if (membership.Price is null)
            {
                return Result<Membership>.Failure("El precio de la membresía es obligatorio.");
            }
            else if (membership.Price <= 0)
            {
                return Result<Membership>.Failure("El precio de la membresía debe ser mayor a cero.");
            }

            if (string.IsNullOrWhiteSpace(membership.Description))
            {
                return Result<Membership>.Failure("La descripción de la membresía es obligatoria.");
            }

            if (membership.MonthlySessions is null)
            {
                return Result<Membership>.Failure("Las sesiones mensuales son obligatorias.");
            }
            else if (membership.MonthlySessions <= 0)
            {
                return Result<Membership>.Failure("Las sesiones mensuales deben ser mayores a cero.");
            }

            return Result<Membership>.Success(membership);
        }
    }
}
