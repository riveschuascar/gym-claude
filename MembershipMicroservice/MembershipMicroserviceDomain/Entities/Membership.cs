namespace MembershipMicroservice.MembershipMicroserviceDomain.Entities
{
    public class Membership
    {
        public short Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public short? MonthlySessions { get; set; }
    }
}