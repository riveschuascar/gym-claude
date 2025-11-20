using LoginMicroservice.Domain.Entities;
using LoginMicroservice.Domain.Rules;
using LoginMicroservice.Domain.Shared;

namespace LoginMicroservice.Domain.Validators
{
    public static class LoginValidator
    {
        public static Result<LoginRequest> Validate(LoginRequest? request)
        {
            if (request == null)
                return Result<LoginRequest>.Failure("Credentials are required.");

            var email = LoginRules.Email(request.Email);
            if (!email.IsSuccess)
                return Result<LoginRequest>.Failure(email.Error!);

            var password = LoginRules.Password(request.Password);
            if (!password.IsSuccess)
                return Result<LoginRequest>.Failure(password.Error!);

            return Result<LoginRequest>.Success(request);
        }
    }
}
