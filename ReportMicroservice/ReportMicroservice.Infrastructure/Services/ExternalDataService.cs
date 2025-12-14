using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportMicroservice.Domain.DTO;
using ReportMicroservice.Domain.Ports;
using ReportMicroservice.Domain.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReportMicroservice.Infrastructure.Services
{
    public class ExternalDataService : IExternalDataService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrlSales = "http://localhost:5305/api";
        private const string BaseUrlClients = "http://localhost:5135/api";
        private const string BaseUrlDisciplines = "http://localhost:5098/api";

        public ExternalDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        }

        public async Task<SaleDto> GetSaleByIdAsync(int id, string token)
        {
            SetToken(token);
            var response = await _httpClient.GetAsync($"{BaseUrlSales}/sales/{id}");
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<SaleDto>(
                await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<SaleDetailDto>> GetSaleDetailsBySaleIdAsync(int saleId, string token)
        {
            SetToken(token);
            // Asumiendo que hay un endpoint para filtrar detalles por venta
            var response = await _httpClient.GetAsync($"{BaseUrlSales}/{saleId}");
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<List<SaleDetailDto>>(
                await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ClientDto> GetClientByIdAsync(int id, string token)
        {
            SetToken(token);
            var response = await _httpClient.GetAsync($"{BaseUrlClients}/clients/{id}");
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<ClientDto>(
                await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<DisciplineDto>> GetAllDisciplinesAsync(string token)
        {
            SetToken(token);
            var response = await _httpClient.GetAsync($"{BaseUrlDisciplines}/disciplines");
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<List<DisciplineDto>>(
                await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
