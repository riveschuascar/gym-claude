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
            var context = _httpContextAccessor.HttpContext;
            string token = context?.Request.Headers["Authorization"];
            string userEmail = context?.User.FindFirst(ClaimTypes.Email)?.Value ?? "system@gmail.com";

            if (string.IsNullOrEmpty(token)) throw new UnauthorizedAccessException("No JWT token found");

            var sale = await _externalDataService.GetSaleByIdAsync(saleId, token);
            var details = sale.Details; 

            var clientTask = _externalDataService.GetClientByIdAsync(sale.ClientId, token);
            var disciplinesTask = _externalDataService.GetAllDisciplinesAsync(token);

            await Task.WhenAll(clientTask, disciplinesTask);

            var client = clientTask.Result;
            var disciplines = disciplinesTask.Result;

            // Lógica con LINQ para armar el modelo del reporte
            var reportDetails = (from d in details
                                 join disc in disciplines on d.DisciplineId equals disc.Id
                                 select new SaleReportDetail
                                 {
                                     Quantity = d.Qty,
                                     Description = disc.Name,
                                     UnitPrice = disc.Price, 
                                     Import = d.Total
                                 }).ToList();

            var reportData = new SaleReportData
            {
                SaleId = sale.Id,
                Date = sale.SaleDate,
                ClientName = client.FullName,
                ClientCiNit = client.Ci, 
                Details = reportDetails,
                TotalAmount = sale.TotalAmount,
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