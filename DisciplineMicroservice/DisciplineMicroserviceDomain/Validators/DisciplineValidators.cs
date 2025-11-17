using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;
using System.Text.RegularExpressions;

namespace DisciplineMicroservice.DisciplineMicroserviceDomain.Validators
{
    public static class DisciplineValidators
    {
        private static readonly Regex AllowedCharsRegex =
            new Regex("^[a-zA-Z0-9 ñáéíóúÁÉÍÓÚüÜ]+$", RegexOptions.Compiled);

        public static Result<Discipline> Validate(Discipline discipline)
        {
            if (discipline == null)
                return Result<Discipline>.Failure("La disciplina no puede quedar vacía.");

            if (string.IsNullOrWhiteSpace(discipline.Name))
                return Result<Discipline>.Failure("El nombre de la disciplina es obligatorio.");

            if (discipline.Name.Length > 20)
                return Result<Discipline>.Failure("El nombre no puede exceder los 20 caracteres.");

            if (!AllowedCharsRegex.IsMatch(discipline.Name))
                return Result<Discipline>.Failure("El nombre contiene caracteres no permitidos.");

            int letterCount = discipline.Name.Count(char.IsLetter);
            int digitCount = discipline.Name.Count(char.IsDigit);

            if (digitCount > 2)
                return Result<Discipline>.Failure("El nombre no puede contener más de 2 números.");

            if (digitCount > 0 && letterCount < 5)
                return Result<Discipline>.Failure("Si el nombre contiene números, debe tener al menos 5 letras.");

            if (letterCount < 3)
                return Result<Discipline>.Failure("El nombre debe contener al menos 3 letras.");

            if (discipline.StartTime == null || discipline.EndTime == null)
                return Result<Discipline>.Failure("Debe especificar hora de inicio y fin.");

            var start = discipline.StartTime.Value;
            var end = discipline.EndTime.Value;

            if (end < start)
                return Result<Discipline>.Failure("La hora de finalización no puede ser anterior a la de inicio.");

            if (end == start)
                return Result<Discipline>.Failure("La hora de inicio y finalización no pueden ser iguales.");

            var duration = end - start;

            if (duration < TimeSpan.FromHours(1))
                return Result<Discipline>.Failure("La disciplina debe durar al menos 1 hora.");

            if (duration > TimeSpan.FromHours(2))
                return Result<Discipline>.Failure("La disciplina no puede durar más de 2 horas.");

            var horaApertura = new TimeSpan(8, 0, 0);
            var horaCierre = new TimeSpan(19, 0, 0);

            if (start < horaApertura || end > horaCierre)
                return Result<Discipline>.Failure("La disciplina debe realizarse entre las 08:00 AM y las 07:00 PM.");

            return Result<Discipline>.Success(discipline);
        }
    }
}