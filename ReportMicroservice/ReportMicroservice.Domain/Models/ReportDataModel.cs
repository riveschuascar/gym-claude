namespace ReportMicroservice.Domain.Models
{
    public class SaleReportData
    {
        public int SaleId { get; set; }
        public DateTime Date { get; set; }
        public string ClientName { get; set; }
        public string ClientCiNit { get; set; }
        public List<SaleReportDetail> Details { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string GeneratedByEmail { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class SaleReportDetail
    {
        public int Quantity { get; set; }
        public string Description { get; set; } // Nombre disciplina
        public decimal UnitPrice { get; set; }
        public decimal Import { get; set; }
    }
}
