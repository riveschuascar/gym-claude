using System.Text.Json.Serialization;

namespace ReportMicroservice.Domain.DTO
{
    public class SaleDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("saleId")]
        public int SaleId { get; set; }

        [JsonPropertyName("disciplineId")] 
        public int DisciplineId { get; set; }

        [JsonPropertyName("qty")] 
        public int Qty { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }
    }
}