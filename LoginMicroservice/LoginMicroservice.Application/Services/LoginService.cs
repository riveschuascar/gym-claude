using LoginMicroservice.Application.Interfaces;
using LoginMicroservice.Application.Models;
using LoginMicroservice.Domain.Entities;
using LoginMicroservice.Domain.Ports;
using LoginMicroservice.Domain.Shared;
using LoginMicroservice.Domain.Validators;

namespace LoginMicroservice.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repository;
        private readonly ITokenProvider _tokenProvider;

        public LoginService(ILoginRepository repository, ITokenProvider tokenProvider)
        {
            _repository = repository;
            _tokenProvider = tokenProvider;
        }

        public async Task<Result<AuthResponse>> AuthenticateAsync(LoginRequest request)
        {
            var validation = LoginValidator.Validate(request);
            if (!validation.IsSuccess)
                return Result<AuthResponse>.Failure(validation.Error!);

            var userResult = await _repository.AuthenticateAsync(request.Email, request.Password);
            if (!userResult.IsSuccess || userResult.Value is null)
                return Result<AuthResponse>.Failure(userResult.Error ?? "Invalid credentials.");

            var token = _tokenProvider.GenerateToken(userResult.Value, out var expiresAt);

            var response = new AuthResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new LoginUserDto
                {
                    Id = userResult.Value.Id,
                    Name = userResult.Value.FullName,
                    Email = userResult.Value.Email,
                    Role = userResult.Value.Role,
                    MustChangePassword = userResult.Value.MustChangePassword
                }
            };

            return Result<AuthResponse>.Success(response);
        }
    }
}
