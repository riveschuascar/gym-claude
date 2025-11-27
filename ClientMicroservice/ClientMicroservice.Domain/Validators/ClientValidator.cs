using System.Text.RegularExpressions;
using ClientMicroservice.Domain.Entities;

namespace ClientMicroservice.Domain.Validators;

public static class ClientValidator
{
    private static readonly Regex NameRegex = new("^[\\p{L}][\\p{L} '\\-]{1,48}$");
    private static readonly Regex CiRegex = new("^[A-Za-z0-9\\-]{5,20}$");
    private static readonly Regex PhoneRegex = new("^[0-9+\\-\\s]{7,20}$");

    public static (bool IsValid, string? Error) Validate(Client client)
    {
        if (client == null)
            return (false, "El cliente no puede ser nulo.");

        client.Name = client.Name?.Trim();
        client.FirstLastname = client.FirstLastname?.Trim();
        client.SecondLastname = client.SecondLastname?.Trim();
        client.Ci = client.Ci?.Trim();
        client.FitnessLevel = client.FitnessLevel?.Trim();
        client.EmergencyContactPhone = client.EmergencyContactPhone?.Trim();

        if (string.IsNullOrWhiteSpace(client.Name))
            return (false, "El nombre es obligatorio.");
        if (client.Name.Length is < 2 or > 50)
            return (false, "El nombre debe tener entre 2 y 50 caracteres.");
        if (!NameRegex.IsMatch(client.Name))
            return (false, "El nombre solo puede contener letras, espacios y guiones.");

        if (string.IsNullOrWhiteSpace(client.FirstLastname))
            return (false, "El primer apellido es obligatorio.");
        if (client.FirstLastname.Length is < 2 or > 50)
            return (false, "El primer apellido debe tener entre 2 y 50 caracteres.");
        if (!NameRegex.IsMatch(client.FirstLastname))
            return (false, "El primer apellido solo puede contener letras, espacios y guiones.");

        if (!string.IsNullOrWhiteSpace(client.SecondLastname) && !NameRegex.IsMatch(client.SecondLastname))
        {
            if (client.SecondLastname.Length is < 2 or > 50)
                return (false, "El segundo apellido debe tener entre 2 y 50 caracteres.");
            return (false, "El segundo apellido solo puede contener letras, espacios y guiones.");
        }

        if (string.IsNullOrWhiteSpace(client.Ci))
            return (false, "La cédula de identidad es obligatoria.");
        if (!CiRegex.IsMatch(client.Ci))
            return (false, "La cédula de identidad solo puede contener letras, números y guiones.");
        if (client.Ci.Length is < 5 or > 20)
            return (false, "La cédula de identidad debe tener entre 5 y 20 caracteres.");

        if (client.DateBirth is null)
            return (false, "La fecha de nacimiento es obligatoria.");
        if (!IsAgeValid(client.DateBirth.Value))
            return (false, "La edad del cliente debe estar entre 18 y 80 años.");

        if (string.IsNullOrWhiteSpace(client.FitnessLevel))
            return (false, "El nivel de condición física es obligatorio.");
        if (client.FitnessLevel.Length is < 2 or > 30)
            return (false, "El nivel de condición física debe tener entre 2 y 30 caracteres.");

        if (client.InitialWeightKg is null)
            return (false, "El peso inicial es obligatorio.");
        if (client.InitialWeightKg < 30 || client.InitialWeightKg > 300)
            return (false, "El peso inicial debe estar entre 30 y 300 kg.");

        if (client.CurrentWeightKg is null)
            return (false, "El peso actual es obligatorio.");
        if (client.CurrentWeightKg <= 0 || client.CurrentWeightKg > 300)
            return (false, "El peso actual debe ser mayor que 0 y no exceder los 300 kg.");

        if (string.IsNullOrWhiteSpace(client.EmergencyContactPhone))
            return (false, "El teléfono de emergencia es obligatorio.");
        if (!PhoneRegex.IsMatch(client.EmergencyContactPhone))
            return (false, "El teléfono de emergencia debe contener al menos 7 dígitos y solo puede incluir números, '+' o '-'.");

        return (true, null);
    }

    private static bool IsAgeValid(DateTime dateBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateBirth.Year;
        if (dateBirth.Date > today.AddYears(-age)) age--;
        return age is >= 18 and <= 80;
    }
}
