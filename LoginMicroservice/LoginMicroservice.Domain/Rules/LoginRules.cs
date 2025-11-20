using System.Text.RegularExpressions;
using LoginMicroservice.Domain.Shared;

namespace LoginMicroservice.Domain.Rules
{
    public static partial class LoginRules
    {
        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        public static Result<string> Email(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<string>.Failure("Email is required.");

            if (!EmailRegex().IsMatch(email))
                return Result<string>.Failure("Email format is not valid.");

            return Result<string>.Success(email);
        }

        public static Result<string> Password(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return Result<string>.Failure("Password is required.");

            // Seed data usa contraseñas de 5 caracteres (ej. "admin"), bajamos a 5 para ser compatibles.
            if (password.Length < 5)
                return Result<string>.Failure("Password must be at least 5 characters.");

            return Result<string>.Success(password);
        }
    }
}
