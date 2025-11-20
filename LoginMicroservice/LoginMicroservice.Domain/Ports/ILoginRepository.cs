using LoginMicroservice.Domain.Entities;
using LoginMicroservice.Domain.Shared;

namespace LoginMicroservice.Domain.Ports
{
    public interface ILoginRepository
    {
        Task<Result<AuthenticatedUser>> AuthenticateAsync(string email, string password);
    }
}
