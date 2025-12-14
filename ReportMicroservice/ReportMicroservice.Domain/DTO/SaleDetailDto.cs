namespace ReportMicroservice.Domain.DTO
{
    public class SaleDetailDto
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int DisciplineId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
