using SalesMicroserviceDomain.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SalesMicroserviceDomain.Events
{
    public class SaleCreatedEvent
    {
        public int SaleId { get; init; }
        public int ClientId { get; init; }
        public List<SaleCreatedEventDetail>? Details { get; init; }
        public DateTime SaleDate { get; init; }
        public decimal TotalAmount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string? TaxId { get; init; }
        public string? BusinessName { get; init; }
        public string? Notes { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? CorrelationId { get; init; }
        public string? OperationId { get; init; }

        public static SaleCreatedEvent FromSale(Sale sale, string? correlationId = null, string? operationId = null)
        {
            return new SaleCreatedEvent
            {
                SaleId = sale.Id ?? 0,
                ClientId = sale.ClientId,
                Details = sale.Details?.Select(d => new SaleCreatedEventDetail
                {
                    DisciplineId = d.DisciplineId,
                    Qty = d.Qty,
                    Price = d.Price,
                    Total = d.Total,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate
                }).ToList(),
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                PaymentMethod = sale.PaymentMethod,
                TaxId = sale.TaxId,
                BusinessName = sale.BusinessName,
                Notes = sale.Notes,
                CreatedBy = sale.CreatedBy,
                CreatedAt = sale.CreatedAt ?? DateTime.UtcNow,
                CorrelationId = correlationId,
                OperationId = operationId
            };
        }
    }

    public class SaleCreatedEventDetail
    {
        public int DisciplineId { get; init; }
        public int Qty { get; init; }
        public decimal Price { get; init; }
        public decimal Total { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
    }
}
