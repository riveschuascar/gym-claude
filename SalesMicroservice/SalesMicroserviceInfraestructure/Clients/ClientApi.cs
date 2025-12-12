using System.Net;
using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Shared;
using Microsoft.Extensions.Logging;

namespace SalesMicroserviceInfraestructure.Clients
{
    public class ClientApi : IClientApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientApi> _logger;

        public ClientApi(HttpClient httpClient, ILogger<ClientApi> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result> EnsureExists(int clientId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Client/{clientId}", cancellationToken);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Result.Failure("El cliente no existe en el microservicio de clientes.");

                response.EnsureSuccessStatusCode();
                return Result.Success();
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Timeout al validar cliente {ClientId}. Se permite continuar de forma optimista.", clientId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo validar cliente {ClientId} en Client API. Se continua optimistamente.", clientId);
                return Result.Success();
            }
        }
    }
}
