using LoginMicroservice.Application.Models;
using LoginMicroservice.Domain.Entities;
using LoginMicroservice.Domain.Shared;

namespace LoginMicroservice.Application.Interfaces
{
    public interface ILoginService
    {
        Task<Result<AuthResponse>> AuthenticateAsync(LoginRequest request);
    }
}
