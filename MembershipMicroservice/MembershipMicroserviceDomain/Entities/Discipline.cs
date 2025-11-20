namespace MembershipMicroservice.MembershipMicroserviceDomain.Entities
{
    public class Discipline
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}