using Dapper;
using Npgsql;
using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Ports;
using SalesMicroserviceDomain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SalesMicroserviceInfraestructure.Persistence
{
    public class SaleRepository : ISaleRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SaleRepository> _logger;

        public SaleRepository(IConfiguration configuration, ILogger<SaleRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("SalesMicroserviceDB")
                ?? throw new Exception("No se encontro la cadena de conexion 'SalesMicroserviceDB'");
            _logger = logger;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<Result<IEnumerable<Sale>>> GetAll()
        {
            const string query = @"
                SELECT id, client_id AS ClientId, membership_id AS MembershipId,
                       start_date AS StartDate, end_date AS EndDate, sale_date AS SaleDate,
                       total_amount AS TotalAmount, payment_method AS PaymentMethod,
                       tax_id AS TaxId, business_name AS BusinessName,
                       notes AS Notes, created_at AS CreatedAt, last_modification AS LastModification,
                       is_active AS IsActive, created_by AS CreatedBy, modified_by AS ModifiedBy
                  FROM membership_sale
                 WHERE is_active = true
                 ORDER BY created_at DESC;";
            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var data = await conn.QueryAsync<Sale>(query);
                return Result<IEnumerable<Sale>>.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll");
                return Result<IEnumerable<Sale>>.Failure("Error al obtener ventas.");
            }
        }

        public async Task<Result<Sale>> GetById(int id)
        {
            const string query = @"
                SELECT id, client_id AS ClientId, membership_id AS MembershipId,
                       start_date AS StartDate, end_date AS EndDate, sale_date AS SaleDate,
                       total_amount AS TotalAmount, payment_method AS PaymentMethod,
                       tax_id AS TaxId, business_name AS BusinessName,
                       notes AS Notes, created_at AS CreatedAt, last_modification AS LastModification,
                       is_active AS IsActive, created_by AS CreatedBy, modified_by AS ModifiedBy
                  FROM membership_sale
                 WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var sale = await conn.QuerySingleOrDefaultAsync<Sale>(query, new { Id = id });
                return sale == null
                    ? Result<Sale>.Failure("Venta no encontrada.")
                    : Result<Sale>.Success(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetById");
                return Result<Sale>.Failure("Error al obtener la venta.");
            }
        }

        public async Task<Result<Sale>> Create(Sale sale, string? userEmail = null)
        {
            const string query = @"
                INSERT INTO membership_sale
                    (client_id, membership_id, start_date, end_date, sale_date, total_amount, payment_method, tax_id, business_name, notes,
                     created_at, last_modification, is_active, created_by)
                VALUES
                    (@ClientId, @MembershipId, @StartDate, @EndDate, @SaleDate, @TotalAmount, @PaymentMethod, @TaxId, @BusinessName, @Notes,
                     @CreatedAt, @LastModification, @IsActive, @CreatedBy)
                RETURNING id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                sale.CreatedBy = userEmail;

                var id = await conn.ExecuteScalarAsync<int>(query, sale);
                sale.Id = id;

                return Result<Sale>.Success(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Create");
                return Result<Sale>.Failure("Error al registrar la venta.");
            }
        }

        public async Task<Result<Sale>> Update(Sale sale, string? userEmail = null)
        {
            const string query = @"
                UPDATE membership_sale
                   SET client_id = @ClientId,
                       membership_id = @MembershipId,
                       start_date = @StartDate,
                       end_date = @EndDate,
                       total_amount = @TotalAmount,
                       payment_method = @PaymentMethod,
                       tax_id = @TaxId,
                       business_name = @BusinessName,
                       notes = @Notes,
                       last_modification = @LastModification,
                       modified_by = @ModifiedBy
                 WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                sale.ModifiedBy = userEmail;

                var affected = await conn.ExecuteAsync(query, sale);
                if (affected == 0)
                    return Result<Sale>.Failure("No se encontro la venta para actualizar.");

                return Result<Sale>.Success(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Update");
                return Result<Sale>.Failure("Error al actualizar la venta.");
            }
        }

        public async Task<Result> Delete(int id, string? userEmail = null)
        {
            const string query = @"
                UPDATE membership_sale
                   SET is_active = false,
                       last_modification = @LastModification,
                       modified_by = @ModifiedBy
                 WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var affected = await conn.ExecuteAsync(query, new
                {
                    Id = id,
                    LastModification = DateTime.UtcNow,
                    ModifiedBy = userEmail
                });

                return affected > 0 ? Result.Success() : Result.Failure("No se encontro la venta para eliminar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Delete");
                return Result.Failure("Error al eliminar la venta.");
            }
        }
    }
}
