using System.Net;
using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Shared;
using Microsoft.Extensions.Logging;

namespace SalesMicroserviceInfraestructure.Clients
{
    public class MembershipApi : IMembershipApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MembershipApi> _logger;

        public MembershipApi(HttpClient httpClient, ILogger<MembershipApi> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result> EnsureExists(int membershipId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Memberships/{membershipId}", cancellationToken);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Result.Failure("La membresía no existe en el microservicio de membresías.");

                response.EnsureSuccessStatusCode();
                return Result.Success();
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Timeout al validar membresía {MembershipId}. Se permite continuar de forma optimista.", membershipId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo validar membresía {MembershipId} en Membership API. Se continua optimistamente.", membershipId);
                return Result.Success();
            }
        }
    }
}
