namespace WebUI.DTO
{
    public class MembershipDTO
    {
        public short Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public short? MonthlySessions { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
    }
}
