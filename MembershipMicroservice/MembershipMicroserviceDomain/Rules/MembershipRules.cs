using System.Text.RegularExpressions;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;

namespace MembershipMicroservice.MembershipMicroserviceDomain.Rules
{
    public static partial class MembershipRules
    {
        [GeneratedRegex(@"^[a-zA-Z0-9 Ò·ÈÌÛ˙¡…Õ”⁄¸‹]+$")]
        private static partial Regex AllowedCharsRegex();
        private static readonly Regex LettersAndNumbersRegex = AllowedCharsRegex();

        public static Result<string> NameRules(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<string>.Failure("El nombre es obligatorio.");

            if (name.Length < 3)
                return Result<string>.Failure("El nombre debe tener al menos 3 caracteres.");

            if (!LettersAndNumbersRegex.IsMatch(name))
                return Result<string>.Failure("El nombre contiene caracteres inv·lidos.");

            return Result<string>.Success(name);
        }

        public static Result<decimal> PriceRules(decimal? price)
        {
            if (!price.HasValue)
                return Result<decimal>.Failure("El precio es obligatorio.");

            if (price.Value <= 0)
                return Result<decimal>.Failure("El precio debe ser mayor a 0.");

            return Result<decimal>.Success(price.Value);
        }

        public static Result<string> DescriptionRules(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Result<string>.Failure("La descripciÛn es obligatoria.");

            if (description.Length < 10)
                return Result<string>.Failure("La descripciÛn debe tener al menos 10 caracteres.");

            return Result<string>.Success(description);
        }

        public static Result<int> MonthlySessionsRules(int? sessions)
        {
            if (!sessions.HasValue)
                return Result<int>.Failure("Las sesiones mensuales son obligatorias.");

            if (sessions.Value < 0)
                return Result<int>.Failure("Las sesiones no pueden ser negativas.");

            return Result<int>.Success(sessions.Value);
        }
    }
}