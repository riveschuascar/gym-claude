using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Report
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public int SaleId { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostGenerateAsync()
        {
            if (SaleId <= 0)
            {
                ErrorMessage = "Por favor ingrese un ID de venta válido.";
                return Page();
            }

            try
            {
                // 1. Usamos el cliente con nombre "ReportAPI" definido en Program.cs
                // Esto automáticamente usa la URL base y el TokenMessageHandler
                var client = _httpClientFactory.CreateClient("ReportAPI");

                // 2. Llamamos al endpoint (la URL base ya está configurada)
                var response = await client.GetAsync($"/api/reports/sale/{SaleId}");

                if (!response.IsSuccessStatusCode)
                {
                    // Si falla (ej. 401 Unauthorized), el TokenMessageHandler no pudo adjuntar el token 
                    // o el token expiró.
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ErrorMessage = "No autorizado. Su sesión puede haber expirado.";
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        ErrorMessage = $"Error al generar reporte: {response.StatusCode}";
                    }
                    return Page();
                }

                // 3. Obtener los bytes del PDF
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                // 4. Devolver el archivo
                return File(pdfBytes, "application/pdf", $"Comprobante_Venta_{SaleId}.pdf");

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocurrió un error de conexión: {ex.Message}";
                return Page();
            }
        }
    }
}