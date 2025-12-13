using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Sales;

public class CreateModel : PageModel
{
    private readonly HttpClient _salesClient;
    private readonly HttpClient _clientApi;
    private readonly HttpClient _disciplineApi;

    public List<ClientDto> Clients { get; set; } = new();
    public List<DisciplineDTO> Disciplines { get; set; } = new();

    [BindProperty]
    public SaleInput Input { get; set; } = new();

    [BindProperty]
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    [BindProperty]
    public string OperationId { get; set; } = Guid.NewGuid().ToString();

    [BindProperty(SupportsGet = true)]
    public string? ClientSearch { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DisciplineSearch { get; set; }

    public CreateModel(IHttpClientFactory factory)
    {
        _salesClient = factory.CreateClient("SalesAPI");
        _clientApi = factory.CreateClient("ClientAPI");
        _disciplineApi = factory.CreateClient("Disciplines");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadLookupsAsync();
        Input.SaleDate = DateTime.Today;
        EnsureCorrelation();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadLookupsAsync();
        EnsureCorrelation();

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Revisa los datos ingresados.";
            return Page();
        }

        // Si no se digitó monto, calcula el total desde los detalles (disciplinas)
        // If details provided, calculate total from details; otherwise leave as provided
        if (Input.Details != null && Input.Details.Any())
        {
            Input.TotalAmount = Input.Details.Sum(d => d.Total);
        }

        try
        {
            var ctx = new
            {
                correlationId = CorrelationId,
                operationId = OperationId
            };

            var payload = new
            {
                clientId = Input.ClientId,
                saleDate = Input.SaleDate,
                totalAmount = Input.TotalAmount,
                nit = Input.TaxId,
                details = Input.Details?.Select(d => new
                {
                    disciplineId = d.DisciplineId,
                    qty = d.Qty,
                    price = d.Price,
                    total = d.Total,
                    startDate = d.StartDate,
                    endDate = d.EndDate
                })
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Sales")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.TryAddWithoutValidation("X-Correlation-Id", ctx.correlationId);
            request.Headers.TryAddWithoutValidation("X-Operation-Id", ctx.operationId);

            var resp = await _salesClient.SendAsync(request);
            var respBody = await resp.Content.ReadAsStringAsync();

            if (resp.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Venta registrada correctamente.";
                return RedirectToPage("Index");
            }

            Console.WriteLine($"Sales API returned {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {respBody}");
            TempData["ErrorMessage"] = $"No se pudo registrar la venta: {(int)resp.StatusCode} {resp.ReasonPhrase} - {respBody}";
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio de ventas.";
            Console.WriteLine("Error al crear venta: " + ex.Message);
            return Page();
        }
    }

    private async Task LoadLookupsAsync()
    {
        try
        {
            Clients = await _clientApi.GetFromJsonAsync<List<ClientDto>>("/api/Client") ?? new List<ClientDto>();
        }
        catch
        {
            Clients = new List<ClientDto>();
        }

        try
        {
            Disciplines = await _discipline_api_GetAll();
        }
        catch
        {
            Disciplines = new List<DisciplineDTO>();
        }

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (!string.IsNullOrWhiteSpace(ClientSearch))
        {
            var term = ClientSearch.Trim().ToLowerInvariant();
            Clients = Clients
                .Where(c =>
                    $"{c.Name} {c.FirstLastname} {c.SecondLastname}".ToLower().Contains(term)
                    || (c.Ci ?? string.Empty).ToLower().Contains(term))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(DisciplineSearch))
        {
            var term = DisciplineSearch.Trim().ToLowerInvariant();
            Disciplines = Disciplines
                .Where(m =>
                    (m.Name ?? string.Empty).ToLower().Contains(term)
                    || (m.Name ?? string.Empty).ToLower().Contains(term))
                .ToList();
        }
    }

    private void EnsureCorrelation()
    {
        if (string.IsNullOrWhiteSpace(CorrelationId))
            CorrelationId = Guid.NewGuid().ToString();

        if (string.IsNullOrWhiteSpace(OperationId))
            OperationId = CorrelationId;
    }

    private async Task<List<DisciplineDTO>> _discipline_api_GetAll()
    {
        try
        {
            return await _disciplineApi.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines") ?? new List<DisciplineDTO>();
        }
        catch
        {
            return new List<DisciplineDTO>();
        }
    }
}

public class SaleInput
{
    [Required(ErrorMessage = "El cliente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
    public int ClientId { get; set; }

    public List<SaleDetailInput>? Details { get; set; }

    [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime SaleDate { get; set; }


    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal TotalAmount { get; set; }

    [StringLength(50, ErrorMessage = "El NIT/CI no puede exceder 50 caracteres.")]
    public string? TaxId { get; set; }
}

public class SaleDetailInput
{
    public int? Id { get; set; }
    public int DisciplineId { get; set; }
    public int Qty { get; set; } = 1;
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

