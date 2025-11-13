using System.Text.RegularExpressions;
using Users.Domain.Shared;

namespace Users.Domain.Rules
{
    internal static partial class UserRules
    {
        [GeneratedRegex(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$")]
        private static partial Regex MyRegex();
        private static readonly Regex LettersAndSpacesRegex = MyRegex();

        public static Result<string> NameRules(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<string>.Failure("El nombre completo es obligatorio.");

            if (name.Length < 2)
                return Result<string>.Failure("El nombre completo debe tener al menos 2 caracteres.");

            if (!LettersAndSpacesRegex.IsMatch(name))
                return Result<string>.Failure("El nombre solo puede contener letras y espacios.");

            return Result<string>.Success(name);
        }

        public static Result<string?> LastNameRules(string? secondLastName)
        {
            if (string.IsNullOrWhiteSpace(secondLastName))
                return Result<string?>.Success(null);

            if (secondLastName.Length < 3)
                return Result<string?>.Failure("Debe tener al menos 3 caracteres.");

            if (!LettersAndSpacesRegex.IsMatch(secondLastName))
                return Result<string?>.Failure("Solo puede contener letras y espacios.");

            return Result<string?>.Success(secondLastName);
        }

        public static Result<string> CiRules(string? ci)
        {
            if (string.IsNullOrWhiteSpace(ci))
                return Result<string>.Failure("El CI es obligatorio.");

            if (!Regex.IsMatch(ci, @"^[0-9A-Za-z]{8,}$"))
                return Result<string>.Failure("El CI debe contener solo letras y números, mínimo 8 caracteres.");

            return Result<string>.Success(ci);
        }

        public static Result<DateTime> DateOfBirthRules(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
                return Result<DateTime>.Failure("La fecha de nacimiento es obligatoria.");

            if (birthDate > DateTime.Today)
                return Result<DateTime>.Failure("La fecha de nacimiento no puede ser futura.");

            int age = DateTime.Today.Year - birthDate.Value.Year;
            if (birthDate.Value.AddYears(age) > DateTime.Today) age--;

            if (age < 18)
                return Result<DateTime>.Failure("El usuario debe tener al menos 18 años.");

            if (age > 60)
                return Result<DateTime>.Failure("El usuario no puede tener más de 60 años.");

            return Result<DateTime>.Success(birthDate.Value);
        }

        public static Result<DateTime> HireDateRules(DateTime? hireDate, DateTime? birthDate)
        {
            if (!hireDate.HasValue || !birthDate.HasValue)
                return Result<DateTime>.Failure("Las fechas de contratación y nacimiento son obligatorias.");

            if (hireDate > DateTime.Today)
                return Result<DateTime>.Failure("La fecha de contratación no puede ser futura.");

            if (hireDate <= birthDate)
                return Result<DateTime>.Failure("La fecha de contratación debe ser posterior a la fecha de nacimiento.");

            int age = hireDate.Value.Year - birthDate.Value.Year;
            if (hireDate.Value < birthDate.Value.AddYears(age)) age--;

            if (age < 18)
                return Result<DateTime>.Failure("El empleado debe tener al menos 18 años al ser contratado.");

            return Result<DateTime>.Success(hireDate.Value);
        }

        public static Result<string> SpecializationRules(string? specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                return Result<string>.Failure("La especialización es obligatoria.");

            if (specialization.Length < 3)
                return Result<string>.Failure("La especialización debe tener al menos 3 caracteres.");

            if (!LettersAndSpacesRegex.IsMatch(specialization))
                return Result<string>.Failure("La especialización solo puede contener letras y espacios.");

            return Result<string>.Success(specialization);
        }

        public static Result<decimal> MonthlySalaryRules(decimal? salary)
        {
            if (!salary.HasValue)
                return Result<decimal>.Failure("El salario es obligatorio.");

            if (salary.Value <= 0)
                return Result<decimal>.Failure("El salario no puede ser negativo o cero.");

            return Result<decimal>.Success(salary.Value);
        }

        public static Result<string> EmailRules(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<string>.Failure("El correo electrónico es obligatorio.");

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
                return Result<string>.Failure("El formato del correo electrónico no es válido.");

            return Result<string>.Success(email);
        }

        public static Result<string> PasswordRules(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return Result<string>.Failure("El campo de contraseña no puede estar vacío.");

            if (password.Length < 8)
                return Result<string>.Failure("La contraseña debe tener al menos 8 caracteres.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return Result<string>.Failure("La contraseña debe contener al menos una letra mayúscula.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return Result<string>.Failure("La contraseña debe contener al menos una letra minúscula.");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return Result<string>.Failure("La contraseña debe contener al menos un número.");

            if (!Regex.IsMatch(password, @"[\W_]"))
                return Result<string>.Failure("La contraseña debe contener al menos un carácter especial.");

            return Result<string>.Success(password);
        }
    }
}
