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
}
