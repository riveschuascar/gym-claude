using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportMicroservice.Application.Services;

namespace ReportMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ReportGenerationService _reportService;

        public ReportsController(ReportGenerationService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sale/{saleId}")]
        public async Task<IActionResult> GetSaleReport(int saleId)
        {
            try
            {
                var pdfBytes = await _reportService.GenerateSaleReportAsync(saleId);

                // Retornar archivo PDF
                return File(pdfBytes, "application/pdf", $"Venta_{saleId}.pdf");
            }
            catch (Exception ex)
            {
                // Manejar errores (ej. venta no encontrada, microservicio caído)
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        
        [HttpPost("sales")]
        public IActionResult ReceiveSaleNotification([FromBody] SaleNotificationDto request)
        {
            // El orquestador nos avisa que la venta terminó.
            // No devolvemos el PDF aquí, solo decimos "Recibido".
            Console.WriteLine($"[Notificación] Venta {request.SaleId} completada exitosamente.");
            return Ok();
        }
    }
    public class SaleNotificationDto
    {
        public int SaleId { get; set; }
    }
}