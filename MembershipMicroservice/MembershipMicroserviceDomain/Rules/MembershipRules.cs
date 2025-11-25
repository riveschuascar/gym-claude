using System.Text.RegularExpressions;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Rules
{
    public static partial class MembershipRules
    {
        [GeneratedRegex(@"^[A-Za-z?-?0-9'\- ]+$")]
        private static partial Regex AllowedCharsRegex();
        private static readonly Regex LettersAndNumbersRegex = AllowedCharsRegex();

        public static Result<string> NameRules(string? name)
        {
            name = name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return Result<string>.Failure("El nombre es obligatorio.");

            if (name.Length < 3 || name.Length > 60)
                return Result<string>.Failure("El nombre debe tener entre 3 y 60 caracteres.");

            if (!LettersAndNumbersRegex.IsMatch(name))
                return Result<string>.Failure("El nombre contiene caracteres inv?lidos.");

            return Result<string>.Success(name);
        }

        public static Result<decimal> PriceRules(decimal? price)
        {
            if (!price.HasValue)
                return Result<decimal>.Failure("El precio es obligatorio.");

            if (price.Value <= 0 || price.Value > 100000)
                return Result<decimal>.Failure("El precio debe ser mayor a 0 y menor a 100000.");

            return Result<decimal>.Success(price.Value);
        }

        public static Result<string> DescriptionRules(string? description)
        {
            description = description?.Trim();

            if (string.IsNullOrWhiteSpace(description))
                return Result<string>.Failure("La descripci?n es obligatoria.");

            if (description.Length < 10 || description.Length > 400)
                return Result<string>.Failure("La descripci?n debe tener entre 10 y 400 caracteres.");

            return Result<string>.Success(description);
        }

        public static Result<int> MonthlySessionsRules(int? sessions)
        {
            if (!sessions.HasValue)
                return Result<int>.Failure("Las sesiones mensuales son obligatorias.");

            if (sessions.Value < 1 || sessions.Value > 90)
                return Result<int>.Failure("Las sesiones deben estar entre 1 y 90.");

            return Result<int>.Success(sessions.Value);
        }
    }
}
