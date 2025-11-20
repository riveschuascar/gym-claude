using System.Text.RegularExpressions;
using WebUI.Common;
using WebUI.DTO;

namespace WebUI.Validation;

public static class ClientValidationRules
{
    private static readonly Regex OnlyLettersAndSpacesRegex = new("^[a-zA-ZÁÉÍÓÚáéíóúÑñ ]+$");
    private static readonly Regex CiRegex = new("^[A-Za-z0-9\\-]+$");
    private static readonly Regex PhoneRegex = new(@"^[0-9+\-]{7,}$");

    public static Result<ClientDto> Validate(ClientDto client)
    {
        if (client == null)
        {
            return Result<ClientDto>.Failure("El objeto del cliente no puede ser nulo.");
        }

        if (string.IsNullOrWhiteSpace(client.Name))
        {
            return Result<ClientDto>.Failure("El nombre es obligatorio.");
        }
        if (!OnlyLettersAndSpacesRegex.IsMatch(client.Name))
        {
            return Result<ClientDto>.Failure("El nombre solo puede contener letras y espacios.");
        }

        if (string.IsNullOrWhiteSpace(client.FirstLastname))
        {
            return Result<ClientDto>.Failure("El primer apellido es obligatorio.");
        }
        if (!OnlyLettersAndSpacesRegex.IsMatch(client.FirstLastname))
        {
            return Result<ClientDto>.Failure("El primer apellido solo puede contener letras y espacios.");
        }

        if (!string.IsNullOrWhiteSpace(client.SecondLastname) &&
            !OnlyLettersAndSpacesRegex.IsMatch(client.SecondLastname))
        {
            return Result<ClientDto>.Failure("El segundo apellido solo puede contener letras y espacios.");
        }

        if (string.IsNullOrWhiteSpace(client.Ci))
        {
            return Result<ClientDto>.Failure("La cédula de identidad es obligatoria.");
        }
        if (!CiRegex.IsMatch(client.Ci))
        {
            return Result<ClientDto>.Failure("La cédula de identidad solo puede contener letras, números y guiones.");
        }
        if (client.Ci.Length < 5)
        {
            return Result<ClientDto>.Failure("La cédula de identidad debe tener al menos 5 caracteres.");
        }

        if (client.DateBirth == null)
        {
            return Result<ClientDto>.Failure("La fecha de nacimiento es obligatoria.");
        }
        if (!IsAgeValid(client.DateBirth))
        {
            return Result<ClientDto>.Failure("La edad del cliente debe estar entre 18 y 80 años.");
        }

        if (string.IsNullOrWhiteSpace(client.FitnessLevel))
        {
            return Result<ClientDto>.Failure("El nivel de condición física es obligatorio.");
        }

        if (client.InitialWeightKg == null)
        {
            return Result<ClientDto>.Failure("El peso inicial es obligatorio.");
        }
        if (client.InitialWeightKg < 30 || client.InitialWeightKg > 300)
        {
            return Result<ClientDto>.Failure("El peso inicial debe estar entre 30 y 300 kg.");
        }

        if (client.CurrentWeightKg == null)
        {
            return Result<ClientDto>.Failure("El peso actual es obligatorio.");
        }
        if (client.CurrentWeightKg <= 0 || client.CurrentWeightKg > 300)
        {
            return Result<ClientDto>.Failure("El peso actual debe ser mayor que 0 y no exceder los 300 kg.");
        }

        if (string.IsNullOrWhiteSpace(client.EmergencyContactPhone))
        {
            return Result<ClientDto>.Failure("El teléfono de emergencia es obligatorio.");
        }
        if (!PhoneRegex.IsMatch(client.EmergencyContactPhone))
        {
            return Result<ClientDto>.Failure("El teléfono de emergencia debe contener al menos 7 dígitos y solo puede incluir números, '+' o '-'.");
        }

        return Result<ClientDto>.Success(client);
    }

    private static bool IsAgeValid(DateTime? dateBirth)
    {
        if (!dateBirth.HasValue) return false;
        var today = DateTime.Today;
        var age = today.Year - dateBirth.Value.Year;
        if (dateBirth.Value.Date > today.AddYears(-age)) age--;
        return age is >= 18 and <= 80;
    }
}

