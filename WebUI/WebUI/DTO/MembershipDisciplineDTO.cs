namespace WebUI.DTO
{
    public class MembershipDisciplineDTO
    {
        public int? Id { get; set; }
        public int MembershipId { get; set; }
        public int DisciplineId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
    }
}
