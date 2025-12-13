namespace WebUI.DTO
{
    public class SaleDTO
    {
        public int? Id { get; set; }
        public int ClientId { get; set; }
        public DateTime SaleDate { get; set; }
        public List<SaleDetailDTO>? Details { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Nit { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class SaleDetailDTO
    {
        public int? Id { get; set; }
        public int SaleId { get; set; }
        public int DisciplineId { get; set; }
        public int Qty { get; set; } = 1;
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
