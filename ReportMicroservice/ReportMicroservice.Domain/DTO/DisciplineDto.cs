using System.Text.Json.Serialization;

namespace ReportMicroservice.Domain.DTO
{
    public class DisciplineDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}