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
        
        [HttpPost("~/api/Report/sales")] 
        public IActionResult ReceiveSaleNotification([FromBody] SaleNotificationDto request)
        {
            // Solo logueamos que el orquestador terminó.
            // No hacemos nada más porque el PDF lo pide el usuario desde la WebUI.
            Console.WriteLine($"[Orchestrator Notification] Venta {request.SaleId} finalizada correctamente.");
            return Ok();
        }
    }
}
    public class SaleNotificationDto
    {
        public int SaleId { get; set; }
    }