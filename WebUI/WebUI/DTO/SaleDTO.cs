namespace WebUI.DTO
{
    public class SaleDTO
    {
        public int? Id { get; set; }
        public int ClientId { get; set; }
        public int MembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? BusinessName { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
    }
}
