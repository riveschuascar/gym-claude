namespace ClientMicroservice.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModification { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Name { get; set; }
    public string? FirstLastname { get; set; }
    public string? SecondLastname { get; set; }
    public DateTime? DateBirth { get; set; }
    public string? Ci { get; set; }

    public string? FitnessLevel { get; set; }
    public decimal? InitialWeightKg { get; set; }
    public decimal? CurrentWeightKg { get; set; }
    public string? EmergencyContactPhone { get; set; }
}
