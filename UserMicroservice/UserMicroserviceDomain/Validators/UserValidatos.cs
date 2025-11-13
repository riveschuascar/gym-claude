using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Rules;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Domain.Validators
{
    public static class UserValidator
    {
        public static Result<User> Validate(User user)
        {
            if (user == null)
                return Result<User>.Failure("El usuario no puede ser nulo.");

            // Validar nombre
            var nameResult = UserRules.NameRules(user.Name);
            if (!nameResult.IsSuccess)
                return Result<User>.Failure(nameResult.Error!);

            // Validar primer apellido
            var firstLastNameResult = UserRules.LastNameRules(user.FirstLastname);
            if (!firstLastNameResult.IsSuccess)
                return Result<User>.Failure(firstLastNameResult.Error!);

            // Validar segundo apellido
            var secondLastNameResult = UserRules.LastNameRules(user.SecondLastname);
            if (!secondLastNameResult.IsSuccess)
                return Result<User>.Failure(secondLastNameResult.Error!);

            // Validar CI
            var ciResult = UserRules.CiRules(user.Ci);
            if (!ciResult.IsSuccess)
                return Result<User>.Failure(ciResult.Error!);

            // Validar fecha de nacimiento
            var dobResult = UserRules.DateOfBirthRules(user.DateOfBirth);
            if (!dobResult.IsSuccess)
                return Result<User>.Failure(dobResult.Error!);

            // Validar fecha de contratación
            var hireDateResult = UserRules.HireDateRules(user.HireDate, user.DateOfBirth);
            if (!hireDateResult.IsSuccess)
                return Result<User>.Failure(hireDateResult.Error!);

            // Validar especialización
            var specializationResult = UserRules.SpecializationRules(user.Specialization);
            if (!specializationResult.IsSuccess)
                return Result<User>.Failure(specializationResult.Error!);

            // Validar salario
            var salaryResult = UserRules.MonthlySalaryRules(user.MonthlySalary);
            if (!salaryResult.IsSuccess)
                return Result<User>.Failure(salaryResult.Error!);

            // Validar email
            var emailResult = UserRules.EmailRules(user.Email);
            if (!emailResult.IsSuccess)
                return Result<User>.Failure(emailResult.Error!);

            // Validar contraseña
            var passwordResult = UserRules.PasswordRules(user.Password);
            if (!passwordResult.IsSuccess)
                return Result<User>.Failure(passwordResult.Error!);

            // Todo validado correctamente
            return Result<User>.Success(user);
        }
    }
}
