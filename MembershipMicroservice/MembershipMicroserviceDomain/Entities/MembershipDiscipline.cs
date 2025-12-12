namespace MembershipMicroservice.MembershipMicroserviceDomain.Entities
{
    public class MembershipDiscipline
    {
        public int? Id { get; set; }
        public int MembershipId { get; set; }
        public int DisciplineId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
