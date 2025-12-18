using Microsoft.AspNetCore.Http;
using ReportMicroservice.Domain.Ports;
using ReportMicroservice.Domain.Models;
using ReportMicroservice.Domain.DTO;
using ReportMicroservice.Application.Utils;
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
            var context = _httpContextAccessor.HttpContext;
            string token = context?.Request.Headers["Authorization"];
            string userEmail = context?.User.FindFirst(ClaimTypes.Email)?.Value ?? "system@gmail.com";

            if (string.IsNullOrEmpty(token)) throw new UnauthorizedAccessException("No JWT token found");

            var saleTask = _externalDataService.GetSaleByIdAsync(saleId, token);
            var detailsTask = _externalDataService.GetSaleDetailsBySaleIdAsync(saleId, token);
            var disciplinesTask = _externalDataService.GetAllDisciplinesAsync(token);

            var sale = await saleTask;

            var clientTask = _externalDataService.GetClientByIdAsync(sale.ClientId, token);

            // Esperamos a que todo lo demás termine
            await Task.WhenAll(detailsTask, disciplinesTask, clientTask);

            var details = await detailsTask;
            var disciplines = await disciplinesTask;
            var client = await clientTask;

            // Lógica explícita para armar el modelo del reporte y asegurar que no se pierdan filas
            var reportDetails = new List<SaleReportDetail>();

            foreach (var d in details)
            {
                var disc = disciplines.FirstOrDefault(x => x.Id == d.DisciplineId);
                
                // Fix: Si la cantidad es 1 pero el total sugiere más, recalculamos.
                var qty = (d.Qty <= 1 && d.Total > d.Price && d.Price > 0)
                          ? (int)Math.Round(d.Total / d.Price)
                          : d.Qty;

                reportDetails.Add(new SaleReportDetail
                {
                    Quantity = qty,
                    Description = disc != null ? disc.Name : $"Disciplina ID: {d.DisciplineId} (No encontrada)",
                    UnitPrice = d.Price,
                    Import = d.Total
                });
            }

            var reportData = new SaleReportData
            {
                SaleId = sale.Id,
                Date = sale.SaleDate,
                ClientName = client.FullName,
                ClientCiNit = sale.Nit, 
                Details = reportDetails,
                TotalAmount = sale.TotalAmount,
                TotalAmountLiteral = CurrencyConverter.Convertir(sale.TotalAmount),
                GeneratedAt = DateTime.Now,
                GeneratedByEmail = userEmail
            };

            // Usar Builder para generar el PDF
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