namespace SalesMicroserviceDomain.Shared
{
    public class OperationContext
    {
        public string CorrelationId { get; }
        public string OperationId { get; }

        public OperationContext(string? correlationId = null, string? operationId = null)
        {
            CorrelationId = string.IsNullOrWhiteSpace(correlationId)
                ? Guid.NewGuid().ToString()
                : correlationId;

            OperationId = string.IsNullOrWhiteSpace(operationId)
                ? CorrelationId
                : operationId;
        }
    }
}
