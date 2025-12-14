using Microsoft.Extensions.Configuration;
using ReportMicroservice.Domain.DTO;
using ReportMicroservice.Domain.Ports;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ReportMicroservice.Infrastructure.Services
{
    public class ExternalDataService : IExternalDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public ExternalDataService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var cleanToken = token.Replace("Bearer ", "").Trim();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", cleanToken);
            }
        }

        public async Task<SaleDto> GetSaleByIdAsync(int id, string token)
        {
            SetToken(token);
            var baseUrl = _configuration["Microservices:SalesApi"];

            var response = await _httpClient.GetAsync($"{baseUrl}/api/Sales/{id}");

            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<SaleDto>(
                await response.Content.ReadAsStreamAsync(), _jsonOptions);
        }

        public async Task<List<SaleDetailDto>> GetSaleDetailsBySaleIdAsync(int saleId, string token)
        {
            var sale = await GetSaleByIdAsync(saleId, token);

            return sale.Details ?? new List<SaleDetailDto>();
        }

        public async Task<ClientDto> GetClientByIdAsync(int id, string token)
        {
            SetToken(token);
            var baseUrl = _configuration["Microservices:ClientsApi"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/Client/{id}");
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<ClientDto>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
        }

        public async Task<List<DisciplineDto>> GetAllDisciplinesAsync(string token)
        {
            SetToken(token);
            var baseUrl = _configuration["Microservices:DisciplinesApi"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/Disciplines");
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<List<DisciplineDto>>(await response.Content.ReadAsStreamAsync(), _jsonOptions);
        }
    }
}