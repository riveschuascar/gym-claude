namespace UserMicroservice.Domain.Entities
{
    public class User
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FirstLastname { get; set; }
        public string SecondLastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Ci { get; set; }
        public string UserRole { get; set; }
        public DateTime HireDate { get; set; }
        public decimal MonthlySalary { get; set; }
        public string Specialization { get; set; }
        public string? Password { get; private set; }
        public string? Email { get; set; }
        public bool? MustChangePassword { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
        public bool? IsActive { get; set; }

        public void UpdateModificationDate()
        {
            LastModification = DateTime.UtcNow;
        }

        public void CreatePassword()
        {
            Password = "GYMcl@ude" + Ci.ToString();
        }
    }
}
