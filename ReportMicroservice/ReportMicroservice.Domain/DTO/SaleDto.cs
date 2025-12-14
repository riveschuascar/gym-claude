namespace ReportMicroservice.Domain.DTO
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Nit { get; set; }
        public List<SaleDetailDto> Details { get; set; } = new List<SaleDetailDto>();
    }
}
