using DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces;
using DisciplineMicroservice.DisciplineMicroserviceApplication.Services;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Ports;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;
using System.Text.RegularExpressions;

namespace DisciplineMicroservice.DisciplineMicroserviceDomain.Validators
{
    public static class DisciplineValidators
    {
        private static readonly Regex AllowedCharsRegex = new Regex("^[a-zA-Z0-9 ñáéíóúÁÉÍÓÚüÜ]+$");

        public static Result<Discipline> Validate(Discipline discipline)
        {
            if (discipline == null)
            {
                return Result<Discipline>.Failure("La disciplina no puede quedar vacía.");
            }

            // Validación del Nombre 
            if (string.IsNullOrWhiteSpace(discipline.Name))
            {
                return Result<Discipline>.Failure("El nombre de la disciplina es obligatorio.");
            }

            if (discipline.Name.Length > 20)
            {
                return Result<Discipline>.Failure("El nombre de la disciplina no puede exceder los 20 caracteres.");
            }

            // Chequeo inicial de caracteres no permitidos
            if (!AllowedCharsRegex.IsMatch(discipline.Name))
            {
                return Result<Discipline>.Failure("El nombre de la disciplina contiene caracteres no permitidos.");
            }

            // Contador de las letras y los números en el nombre.
            int letterCount = discipline.Name.Count(char.IsLetter);
            int digitCount = discipline.Name.Count(char.IsDigit);

            // No se permitir más de 2 números.
            if (digitCount > 2)
            {
                return Result<Discipline>.Failure("El nombre no puede contener más de 2 números.");
            }

            // Si hay números, debe haber al menos 5 letras.
            if (digitCount > 0 && letterCount < 5)
            {
                return Result<Discipline>.Failure("Si el nombre contiene números, debe estar acompañado por al menos 5 letras.");
            }

            // Regla general de un mínimo de letras,
            if (letterCount < 3)
            {
                return Result<Discipline>.Failure("El nombre debe contener al menos 3 letras.");
            }


            //Validación de Horarios
            var duration = discipline.EndTime - discipline.StartTime;
            if (discipline.EndTime < discipline.StartTime)
            {
                return Result<Discipline>.Failure("La hora de finalización no puede ser anterior a la hora de inicio.");
            }
            if (discipline.EndTime == discipline.StartTime)
            {
                return Result<Discipline>.Failure("La hora de inicio y finalización no pueden ser la misma.");
            }
            if (duration < TimeSpan.FromHours(1))
            {
                return Result<Discipline>.Failure("La duración de la disciplina debe ser de al menos 1 hora.");
            }
            if (duration > TimeSpan.FromHours(2))
            {
                return Result<Discipline>.Failure("La duración de la disciplina no puede exceder las 2 horas.");
            }
            TimeSpan HoraApertura = new TimeSpan(8, 0, 0); // 08:00 AM
            TimeSpan HoraCierre = new TimeSpan(19, 0, 0);  // 07:00 PM

            // Comprobamos si la hora de inicio o la de finalización están fuera del rango permitido.
            if (discipline.StartTime < HoraApertura || discipline.EndTime > HoraCierre)
            {
                return Result<Discipline>.Failure("El horario de la disciplina debe estar entre las 08:00 AM y las 07:00 PM.");
            }

            return Result<Discipline>.Success(discipline);
        }
    }
}
