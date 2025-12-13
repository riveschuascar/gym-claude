namespace DisciplineMicroservice.DisciplineMicroserviceDomain.Entities
{
    public class Discipline
    {
        public short Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Name { get; set; }
        public long? IdUser { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public double? Price { get; set; }
        public short? MonthlySessions { get; set; } 
    }
}
