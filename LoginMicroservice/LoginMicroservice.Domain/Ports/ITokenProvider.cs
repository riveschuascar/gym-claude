using LoginMicroservice.Domain.Entities;

namespace LoginMicroservice.Domain.Ports
{
    public interface ITokenProvider
    {
        string GenerateToken(AuthenticatedUser user, out DateTime expiresAt);
    }
}
