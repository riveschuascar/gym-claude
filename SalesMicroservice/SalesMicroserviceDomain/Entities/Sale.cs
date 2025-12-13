using System;
using System.Collections.Generic;

namespace SalesMicroserviceDomain.Entities
{
    public class Sale
    {
        public int? Id { get; set; }
        public int ClientId { get; set; }
        public DateTime SaleDate { get; set; }
        public List<SaleDetail>? Details { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Nit { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModification { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
