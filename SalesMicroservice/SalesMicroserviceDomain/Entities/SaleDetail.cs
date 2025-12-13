using System;

namespace SalesMicroserviceDomain.Entities
{
    public class SaleDetail
    {
        public int? Id { get; set; }
        public int SaleId { get; set; }
        public int DisciplineId { get; set; }
        public int Qty { get; set; } = 1;
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
