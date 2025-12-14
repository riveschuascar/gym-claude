using Microsoft.AspNetCore.Http;
using ReportMicroservice.Domain.Ports;
using ReportMicroservice.Domain.Models;
using System.Security.Claims;

namespace ReportMicroservice.Application.Services
{
    public class ReportGenerationService
    {
        private readonly IExternalDataService _externalDataService;
        private readonly IReportBuilder _reportBuilder;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportGenerationService(
            IExternalDataService externalDataService,
            IReportBuilder reportBuilder,
            IHttpContextAccessor httpContextAccessor)
        {
            _externalDataService = externalDataService;
            _reportBuilder = reportBuilder;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<byte[]> GenerateSaleReportAsync(int saleId)
        {
            // 1. Obtener el Token JWT actual para propagarlo
            var context = _httpContextAccessor.HttpContext;
            string token = context?.Request.Headers["Authorization"];
            string userEmail = context?.User.FindFirst(ClaimTypes.Email)?.Value ?? "elad@gmail.com";

            if (string.IsNullOrEmpty(token)) throw new UnauthorizedAccessException("No JWT token found");

            // 2. Obtener datos externos (paralelizar donde sea posible)
            // Primero la venta para saber quién es el cliente
            var sale = await _externalDataService.GetSaleByIdAsync(saleId, token);

            var clientTask = _externalDataService.GetClientByIdAsync(sale.ClientId, token);
            var saleDetailsTask = _externalDataService.GetSaleDetailsBySaleIdAsync(saleId, token);
            var disciplinesTask = _externalDataService.GetAllDisciplinesAsync(token); // Traemos todas para mapear nombres

            await Task.WhenAll(clientTask, saleDetailsTask, disciplinesTask);

            var client = clientTask.Result;
            var details = saleDetailsTask.Result;
            var disciplines = disciplinesTask.Result;

            // 3. Lógica con LINQ para armar el modelo del reporte
            // Unimos Detalle Venta con Disciplinas para obtener el nombre
            var reportDetails = (from d in details
                                 join disc in disciplines on d.DisciplineId equals disc.Id
                                 select new SaleReportDetail
                                 {
                                     Quantity = d.Qty,
                                     Description = disc.Name,
                                     UnitPrice = disc.Price, // Precio base de disciplina (o d.Price si hubo descuento en venta)
                                     Import = d.Total
                                 }).ToList();

            var reportData = new SaleReportData
            {
                SaleId = sale.Id,
                Date = sale.SaleDate,
                ClientName = client.FullName,
                ClientCiNit = client.Ci, // Usamos CI del cliente segun requerimiento
                Details = reportDetails,
                TotalAmount = sale.TotalAmount,
                GeneratedAt = DateTime.Now,
                GeneratedByEmail = userEmail
            };

            // 4. Usar el Builder para generar el PDF
            return _reportBuilder
                .Reset()
                .SetReportData(reportData)
                .BuildHeader()
                .BuildCustomerInfo()
                .BuildDetailsTable()
                .BuildTotalSection()
                .BuildFooter()
                .GetPdfBytes();
        }
    }
}