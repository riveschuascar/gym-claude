namespace ReportMicroservice.Domain.DTO
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstLastname { get; set; }
        public string SecondLastname { get; set; }
        public string Ci { get; set; }
        
        public string FullName => $"{Name} {FirstLastname} {SecondLastname}".Trim();
    }

}
