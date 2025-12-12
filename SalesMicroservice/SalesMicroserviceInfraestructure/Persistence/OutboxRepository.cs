using Dapper;
using Npgsql;
using SalesMicroserviceDomain.Ports;
using SalesMicroserviceDomain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SalesMicroserviceInfraestructure.Persistence
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<OutboxRepository> _logger;

        public OutboxRepository(IConfiguration configuration, ILogger<OutboxRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("SalesMicroserviceDB")
                ?? throw new Exception("No se encontro la cadena de conexion 'SalesMicroserviceDB' para Outbox.");
            _logger = logger;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<Result> SaveAsync(OutboxMessage message)
        {
            const string query = @"
                INSERT INTO outbox_messages (id, message_type, payload, occurred_on, correlation_id, operation_id)
                VALUES (@Id, @Type, CAST(@Payload AS jsonb), @OccurredOn, @CorrelationId, @OperationId);";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                await conn.ExecuteAsync(query, new
                {
                    message.Id,
                    message.Type,
                    message.Payload,
                    message.OccurredOn,
                    message.CorrelationId,
                    message.OperationId
                });

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar mensaje en outbox");
                return Result.Failure("No se pudo persistir el evento en outbox.");
            }
        }
    }
}
