namespace SalesMicroserviceDomain.Shared
{
    public class OutboxMessage
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Payload { get; init; } = string.Empty;
        public DateTime OccurredOn { get; init; }
        public string? CorrelationId { get; init; }
        public string? OperationId { get; init; }
    }
}
