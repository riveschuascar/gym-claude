using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Sales
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _salesClient;
        private readonly HttpClient _clientApi;
        private readonly HttpClient _disciplineApi;

        public List<SaleRow> Sales { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public IndexModel(IHttpClientFactory factory)
        {
            _salesClient = factory.CreateClient("SalesAPI");
            _clientApi = factory.CreateClient("ClientAPI");
            _disciplineApi = factory.CreateClient("Disciplines");
        }

        public class SaleRow
        {
            public int Id { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public string Disciplines { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public DateTime SaleDate { get; set; }
            public string? Nit { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var sales = await _salesClient.GetFromJsonAsync<List<SaleDTO>>("/api/Sales") ?? new List<SaleDTO>();
                var clients = await _clientApi.GetFromJsonAsync<List<ClientDto>>("/api/Client") ?? new List<ClientDto>();
                var disciplines = await _disciplineApi.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines") ?? new List<DisciplineDTO>();

                var clientLookup = clients.ToDictionary(c => c.Id, c => $"{c.Name} {c.FirstLastname} {c.SecondLastname}".Trim());
                var disciplineLookup = disciplines.ToDictionary(d => (int)d.Id, d => d.Name ?? "Disciplina");

                var filtered = sales.Where(s => s.IsActive);

                if (!string.IsNullOrWhiteSpace(Search))
                {
                    var term = Search.Trim().ToLowerInvariant();
                    filtered = filtered.Where(s =>
                    {
                        var clientName = clientLookup.TryGetValue(s.ClientId, out var cn) ? cn : string.Empty;
                        var clientCi = clients.FirstOrDefault(c => c.Id == s.ClientId)?.Ci ?? string.Empty;
                        var taxId = s.Nit ?? string.Empty;
                        var disciplineNames = s.Details?.Select(d => disciplineLookup.TryGetValue(d.DisciplineId, out var dn) ? dn : ("Disciplina #" + d.DisciplineId)).ToArray() ?? Array.Empty<string>();
                            return clientName.ToLower().Contains(term)
                                   || clientCi.ToLower().Contains(term)
                                   || taxId.ToLower().Contains(term)
                                   || string.Join(" ", disciplineNames).ToLower().Contains(term);
                    });
                }

                Sales = filtered
                    .Where(s => s.IsActive)
                    .Select(s => new SaleRow
                    {
                        Id = s.Id ?? 0,
                        ClientName = clientLookup.TryGetValue(s.ClientId, out var cn) ? cn : $"Cliente #{s.ClientId}",
                        Disciplines = s.Details != null && s.Details.Any()
                            ? string.Join(", ", s.Details.Select(d => disciplineLookup.TryGetValue(d.DisciplineId, out var dn) ? dn : ("Disciplina #" + d.DisciplineId)))
                            : "-",
                        Total = s.TotalAmount,
                        SaleDate = s.SaleDate,
                        Nit = s.Nit
                    })
                    .OrderByDescending(s => s.SaleDate)
                    .ToList();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToPage("/Login/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No se pudieron cargar las ventas.";
                Console.WriteLine("Error al obtener ventas: " + ex.Message);
                Sales = new List<SaleRow>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var resp = await _salesClient.DeleteAsync($"/api/Sales/{id}");
                TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    resp.IsSuccessStatusCode ? "Venta eliminada." : "No se pudo eliminar la venta.";
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToPage("/Login/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el microservicio de ventas.";
                Console.WriteLine("Error al eliminar venta: " + ex.Message);
            }

            return RedirectToPage();
        }
    }
}
