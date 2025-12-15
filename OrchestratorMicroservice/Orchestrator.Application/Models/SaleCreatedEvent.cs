using System.Collections.Generic;

namespace Orchestrator.Application.Models
{
    public class SaleCreatedEvent
    {
        public long IdOrquestas { get; set; }
        public int SaleId { get; set; }
        public int ClientId { get; set; }
        public List<int> DisciplinesIds { get; set; } = new List<int>();
    }
}
