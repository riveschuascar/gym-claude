using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
        private readonly HttpClient _saleDetailClient;
        private readonly HttpClient _clientApi;
        private readonly HttpClient _disciplineApi;
        private readonly HttpClient _reportClient;

        public List<SaleRow> Sales { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SaleId { get; set; } // El ID que viene del Create

        [BindProperty(SupportsGet = true)]
        public bool AutoDownload { get; set; } // La bandera para saber si descargamos
        public IndexModel(IHttpClientFactory factory)
        {
            _reportClient = factory.CreateClient("ReportAPI");
            _salesClient = factory.CreateClient("SalesAPI");
            _saleDetailClient = factory.CreateClient("SaleDetailAPI");
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
        public async Task<IActionResult> OnGetDownloadAsync(int saleId)
        {
            try
            {
                var response = await _reportClient.GetAsync($"/api/reports/sale/{saleId}");

                if (!response.IsSuccessStatusCode) return NotFound();

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return File(pdfBytes, "application/pdf", $"Comprobante_Venta_{saleId}.pdf");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnGetSaleDetailsAsync(int id)
        {
            try
            {
                var saleTask = _salesClient.GetFromJsonAsync<SaleDTO>($"/api/Sales/{id}");
                
                var detailsTask = _saleDetailClient.GetFromJsonAsync<List<SaleDetailDTO>>($"/api/SaleDetails/sale/{id}");
                
                var disciplinesTask = _disciplineApi.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines");

                await Task.WhenAll(saleTask, detailsTask, disciplinesTask);

                var sale = saleTask.Result;
                if (sale == null) return NotFound();

                sale.Details = detailsTask.Result ?? new List<SaleDetailDTO>();

                string clientName = "Cliente Desconocido";
                try 
                {
                    var client = await _clientApi.GetFromJsonAsync<ClientDto>($"/api/Client/{sale.ClientId}");
                    if (client != null) 
                        clientName = $"{client.Name} {client.FirstLastname} {client.SecondLastname}".Trim();
                }
                catch { /* Ignorar error de cliente */ }

                var disciplines = disciplinesTask.Result;
                var discLookup = disciplines?.ToDictionary(d => (int)d.Id, d => d.Name ?? "Sin Nombre") 
                                ?? new Dictionary<int, string>();

                var detailsList = sale.Details.Select(d => new
                {
                    disciplineName = discLookup.TryGetValue(d.DisciplineId, out var name) ? name : $"Disciplina {d.DisciplineId}",
                    qty = d.Qty,
                    unitPrice = d.Price, 
                    subTotal = d.Total
                }).ToList();

                var response = new
                {
                    id = sale.Id,
                    clientName = clientName,
                    saleDate = sale.SaleDate,
                    nit = sale.Nit,
                    total = sale.TotalAmount,
                    details = detailsList
                };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al cargar detalles: " + ex.Message);
            }
        }
    }
}
