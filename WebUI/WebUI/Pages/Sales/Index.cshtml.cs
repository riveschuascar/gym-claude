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
        private readonly HttpClient _membershipApi;

        public List<SaleRow> Sales { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public IndexModel(IHttpClientFactory factory)
        {
            _salesClient = factory.CreateClient("SalesAPI");
            _clientApi = factory.CreateClient("ClientAPI");
            _membershipApi = factory.CreateClient("Memberships");
        }

        public class SaleRow
        {
            public int Id { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public string MembershipName { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public string Payment { get; set; } = string.Empty;
            public DateTime SaleDate { get; set; }
            public string? TaxId { get; set; }
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var sales = await _salesClient.GetFromJsonAsync<List<SaleDTO>>("/api/Sales") ?? new List<SaleDTO>();
                var clients = await _clientApi.GetFromJsonAsync<List<ClientDto>>("/api/Client") ?? new List<ClientDto>();
                var memberships = await _membershipApi.GetFromJsonAsync<List<MembershipDTO>>("/api/Memberships") ?? new List<MembershipDTO>();

                var clientLookup = clients.ToDictionary(c => c.Id, c => $"{c.Name} {c.FirstLastname} {c.SecondLastname}".Trim());
                var membershipLookup = memberships.ToDictionary(m => (int)(m.Id ?? 0), m => m.Name ?? "Membresía");

                var filtered = sales.Where(s => s.IsActive);

                if (!string.IsNullOrWhiteSpace(Search))
                {
                    var term = Search.Trim().ToLowerInvariant();
                    filtered = filtered.Where(s =>
                    {
                        var clientName = clientLookup.TryGetValue(s.ClientId, out var cn) ? cn : string.Empty;
                        var clientCi = clients.FirstOrDefault(c => c.Id == s.ClientId)?.Ci ?? string.Empty;
                        var taxId = s.TaxId ?? string.Empty;
                        var membershipName = membershipLookup.TryGetValue(s.MembershipId, out var mn) ? mn : string.Empty;
                        return clientName.ToLower().Contains(term)
                               || clientCi.ToLower().Contains(term)
                               || taxId.ToLower().Contains(term)
                               || membershipName.ToLower().Contains(term);
                    });
                }

                Sales = filtered
                    .Where(s => s.IsActive)
                    .Select(s => new SaleRow
                    {
                        Id = s.Id ?? 0,
                        ClientName = clientLookup.TryGetValue(s.ClientId, out var cn) ? cn : $"Cliente #{s.ClientId}",
                        MembershipName = membershipLookup.TryGetValue(s.MembershipId, out var mn) ? mn : $"Membresía #{s.MembershipId}",
                        Total = s.TotalAmount,
                        Payment = s.PaymentMethod,
                        SaleDate = s.SaleDate == default ? s.StartDate : s.SaleDate,
                        TaxId = s.TaxId,
                        Notes = s.Notes
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
