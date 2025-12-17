using System;
using System.Collections.Generic;

namespace Orchestrator.Application.Models
{
    public class SaleCreatedEvent
    {
        public long IdOrquestas { get; set; }
        public int SaleId { get; set; }
        public int ClientId { get; set; }
        public List<SaleDetailDto> Details { get; set; } = new List<SaleDetailDto>();
    }

    public class SaleDetailDto
    {
        public int SaleId { get; set; }
        public int DisciplineId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal? Total { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}