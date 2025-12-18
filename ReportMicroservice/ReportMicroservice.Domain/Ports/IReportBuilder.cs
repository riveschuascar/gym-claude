using ReportMicroservice.Domain.DTO;
namespace ReportMicroservice.Domain.Ports
{
    public interface IReportBuilder
    {
        IReportBuilder Reset();
        IReportBuilder SetReportData(Models.SaleReportData data);
        IReportBuilder BuildHeader();
        IReportBuilder BuildCustomerInfo();
        IReportBuilder BuildDetailsTable();
        IReportBuilder BuildTotalSection();
        IReportBuilder BuildFooter();
        byte[] GetPdfBytes();
    }

    // Interfaz para obtener datos externos
    public interface IExternalDataService
    {
        Task<SaleDto> GetSaleByIdAsync(int id, string token);
        Task<List<SaleDetailDto>> GetSaleDetailsBySaleIdAsync(int saleId, string token);
        Task<ClientDto> GetClientByIdAsync(int id, string token);
        Task<List<DisciplineDto>> GetAllDisciplinesAsync(string token);
    }
}
