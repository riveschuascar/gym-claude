using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Rules;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Domain.Validators
{
    public static class UserSpecification
    {
        public static Result<User> Create(User user)
        {
            if (user == null)
                return Result<User>.Failure("El usuario no puede ser nulo.");

            var nameResult = UserRules.NameRules(user.Name);
            if (!nameResult.IsSuccess)
                return Result<User>.Failure(nameResult.Error!);

            var firstLastNameResult = UserRules.LastNameRules(user.FirstLastname);
            if (!firstLastNameResult.IsSuccess)
                return Result<User>.Failure(firstLastNameResult.Error!);

            var secondLastNameResult = UserRules.LastNameRules(user.SecondLastname);
            if (!secondLastNameResult.IsSuccess)
                return Result<User>.Failure(secondLastNameResult.Error!);

            var ciResult = UserRules.CiRules(user.Ci);
            if (!ciResult.IsSuccess)
                return Result<User>.Failure(ciResult.Error!);

            var dobResult = UserRules.DateOfBirthRules(user.DateOfBirth);
            if (!dobResult.IsSuccess)
                return Result<User>.Failure(dobResult.Error!);

            var hireDateResult = UserRules.HireDateRules(user.HireDate, user.DateOfBirth);
            if (!hireDateResult.IsSuccess)
                return Result<User>.Failure(hireDateResult.Error!);

            var specializationResult = UserRules.SpecializationRules(user.Specialization);
            if (!specializationResult.IsSuccess)
                return Result<User>.Failure(specializationResult.Error!);

            var salaryResult = UserRules.MonthlySalaryRules(user.MonthlySalary);
            if (!salaryResult.IsSuccess)
                return Result<User>.Failure(salaryResult.Error!);

            var emailResult = UserRules.EmailRules(user.Email);
            if (!emailResult.IsSuccess)
                return Result<User>.Failure(emailResult.Error!);

            var passwordResult = UserRules.PasswordRules(user.Password);
            if (!passwordResult.IsSuccess)
                return Result<User>.Failure(passwordResult.Error!);

            return Result<User>.Success(user);
        }

        public static Result<User> Update(User user)
        {
            if (user == null)
                return Result<User>.Failure("El usuario no puede ser nulo.");

            var nameResult = UserRules.NameRules(user.Name);
            if (!nameResult.IsSuccess)
                return Result<User>.Failure(nameResult.Error!);

            var firstLastNameResult = UserRules.LastNameRules(user.FirstLastname);
            if (!firstLastNameResult.IsSuccess)
                return Result<User>.Failure(firstLastNameResult.Error!);

            var secondLastNameResult = UserRules.LastNameRules(user.SecondLastname);
            if (!secondLastNameResult.IsSuccess)
                return Result<User>.Failure(secondLastNameResult.Error!);

            var ciResult = UserRules.CiRules(user.Ci);
            if (!ciResult.IsSuccess)
                return Result<User>.Failure(ciResult.Error!);

            var dobResult = UserRules.DateOfBirthRules(user.DateOfBirth);
            if (!dobResult.IsSuccess)
                return Result<User>.Failure(dobResult.Error!);

            var specializationResult = UserRules.SpecializationRules(user.Specialization);
            if (!specializationResult.IsSuccess)
                return Result<User>.Failure(specializationResult.Error!);

            var salaryResult = UserRules.MonthlySalaryRules(user.MonthlySalary);
            if (!salaryResult.IsSuccess)
                return Result<User>.Failure(salaryResult.Error!);

            return Result<User>.Success(user);
        }
    }
}
