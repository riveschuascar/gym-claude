using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using System.Text.RegularExpressions;
using MembershipMicroservice.MembershipMicroserviceDomain.Rules;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Validators
{
    public static class MembershipValidators
    {
        private static readonly Regex AllowedCharsRegex =
            new Regex("^[a-zA-Z0-9 ñáéíóúÁÉÍÓÚüÜ]+$", RegexOptions.Compiled);

        public static Result<Membership> Create(Membership membership)
        {
            if (membership == null)
                return Result<Membership>.Failure("La membresía no puede ser nula.");

            var name = MembershipRules.NameRules(membership.Name);
            if (name.IsFailure) return Result<Membership>.Failure(name.Error!);
            membership.Name = name.Value;

            var price = MembershipRules.PriceRules(membership.Price);
            if (price.IsFailure) return Result<Membership>.Failure(price.Error!);
            membership.Price = price.Value;

            var desc = MembershipRules.DescriptionRules(membership.Description);
            if (desc.IsFailure) return Result<Membership>.Failure(desc.Error!);
            membership.Description = desc.Value;

            var sessionsResult = MembershipRules.MonthlySessionsRules(membership.MonthlySessions ?? 0);
            if (!sessionsResult.IsSuccess)
                return Result<Membership>.Failure(sessionsResult.Error!);
            membership.MonthlySessions = sessionsResult.Value;

            return Result<Membership>.Success(membership);
        }

        public static Result<Membership> Update(Membership membership)
        {
            if (membership == null)
                return Result<Membership>.Failure("La membresía no puede ser nula.");

            if ((membership.Id ?? 0) <= 0)
                return Result<Membership>.Failure("El Id de la membresía debe ser mayor a cero.");

            var nameResult = MembershipRules.NameRules(membership.Name);
            if (!nameResult.IsSuccess)
                return Result<Membership>.Failure(nameResult.Error!);
            membership.Name = nameResult.Value;

            var priceResult = MembershipRules.PriceRules(membership.Price);
            if (!priceResult.IsSuccess)
                return Result<Membership>.Failure(priceResult.Error!);
            membership.Price = priceResult.Value;

            var descriptionResult = MembershipRules.DescriptionRules(membership.Description);
            if (!descriptionResult.IsSuccess)
                return Result<Membership>.Failure(descriptionResult.Error!);
            membership.Description = descriptionResult.Value;

            var sessionsResult = MembershipRules.MonthlySessionsRules(membership.MonthlySessions ?? 0);
            if (!sessionsResult.IsSuccess)
                return Result<Membership>.Failure(sessionsResult.Error!);
            membership.MonthlySessions = sessionsResult.Value;

            return Result<Membership>.Success(membership);
        }
    }
}
