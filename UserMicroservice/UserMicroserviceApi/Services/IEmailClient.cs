using System.Threading.Tasks;

namespace UserMicroservice.Api.Services
{
    public interface IEmailClient
    {
        Task SendCredentialEmailAsync(string email, string userFullName, string reason, string securityRecommendations, string username, string password);
    }
}
