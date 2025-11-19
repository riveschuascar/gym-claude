namespace WebUI.DTO;

public class ClientDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? FirstLastname { get; set; }
    public string? SecondLastname { get; set; }
    public DateTime? DateBirth { get; set; }
    public string? Ci { get; set; }
    public string? FitnessLevel { get; set; }
    public decimal? InitialWeightKg { get; set; }
    public decimal? CurrentWeightKg { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModification { get; set; }
}
