using System.Threading.Tasks;
using EmailMicroservice.API;

namespace UserMicroservice.Api.Services
{
    public class EmailGrpcClient : IEmailClient
    {
        private readonly EmailService.EmailServiceClient _client;

        public EmailGrpcClient(EmailService.EmailServiceClient client)
        {
            _client = client;
        }

        public async Task SendCredentialEmailAsync(string email, string userFullName, string reason, string securityRecommendations, string username, string password)
        {
            var request = new SendCredentialEmailRequest
            {
                Email = email,
                UserFullName = userFullName,
                Reason = reason ?? string.Empty,
                SecurityRecommendations = securityRecommendations ?? string.Empty,
                Username = username ?? string.Empty,
                Password = password ?? string.Empty
            };

            // Fire and forget semantics can be used here, but await to propagate errors if needed
            var response = await _client.SendCredentialEmailAsync(request);

            // Optionally log or handle response.Success/Message
        }
    }
}
