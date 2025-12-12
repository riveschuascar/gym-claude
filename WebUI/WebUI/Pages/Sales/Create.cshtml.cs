using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _salesClient;
        private readonly HttpClient _clientApi;
        private readonly HttpClient _membershipApi;

        public List<ClientDto> Clients { get; set; } = new();
        public List<MembershipDTO> Memberships { get; set; } = new();

        [BindProperty]
        public SaleInput Input { get; set; } = new();

        public CreateModel(IHttpClientFactory factory)
        {
            _salesClient = factory.CreateClient("SalesAPI");
            _clientApi = factory.CreateClient("ClientAPI");
            _membershipApi = factory.CreateClient("Memberships");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadLookupsAsync();
            Input.SaleDate = DateTime.Today;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadLookupsAsync();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Revisa los datos ingresados.";
                return Page();
            }

            // Si no se digitó monto, toma el precio de la membresía
            if (Input.TotalAmount <= 0)
            {
                var membership = Memberships.FirstOrDefault(m => m.Id == Input.MembershipId);
                if (membership?.Price is decimal price && price > 0)
                {
                    Input.TotalAmount = price;
                }
            }

            try
            {
                var payload = new
                {
                    clientId = Input.ClientId,
                    membershipId = Input.MembershipId,
                    startDate = Input.SaleDate,
                    endDate = Input.SaleDate,
                    saleDate = Input.SaleDate,
                    totalAmount = Input.TotalAmount,
                    paymentMethod = Input.PaymentMethod,
                    taxId = Input.TaxId,
                    businessName = Input.BusinessName,
                    notes = Input.Notes
                };

                var resp = await _salesClient.PostAsJsonAsync("/api/Sales", payload);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Venta registrada correctamente.";
                    return RedirectToPage("Index");
                }

                TempData["ErrorMessage"] = "No se pudo registrar la venta.";
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
                Memberships = await _membershipApi.GetFromJsonAsync<List<MembershipDTO>>("/api/Memberships") ?? new List<MembershipDTO>();
            }
            catch
            {
                Memberships = new List<MembershipDTO>();
            }
        }
    }

    public class SaleInput
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "La membresía es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una membresía.")]
        public int MembershipId { get; set; }

        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "Seleccione un método de pago.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal TotalAmount { get; set; }

        [StringLength(50, ErrorMessage = "El NIT/CI no puede exceder 50 caracteres.")]
        public string? TaxId { get; set; }

        [StringLength(200, ErrorMessage = "La razón social no puede exceder 200 caracteres.")]
        public string? BusinessName { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres.")]
        public string? Notes { get; set; }
    }
}
