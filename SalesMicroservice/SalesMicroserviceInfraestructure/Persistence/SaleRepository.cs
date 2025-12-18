using Dapper;
using Npgsql;
using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Ports;
using SalesMicroserviceDomain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
                SELECT s.id, s.client_id AS ClientId, s.sale_date AS SaleDate,
                       s.total_amount AS TotalAmount, s.nit AS Nit,
                       s.created_at AS CreatedAt, s.last_modification AS LastModification,
                       s.is_active AS IsActive, s.created_by AS CreatedBy, s.modified_by AS ModifiedBy
                FROM sales s";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var sales = await conn.QueryAsync<Sale>(query);
                return Result<IEnumerable<Sale>>.Success(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll");
                return Result<IEnumerable<Sale>>.Failure("Error al obtener las ventas.");
            }
        }

        public async Task<Result<Sale>> GetById(int id)
        {
            const string query = @"
                SELECT s.id, s.client_id AS ClientId, s.sale_date AS SaleDate,
                       s.total_amount AS TotalAmount, s.nit AS Nit,
                       s.created_at AS CreatedAt, s.last_modification AS LastModification,
                       s.is_active AS IsActive, s.created_by AS CreatedBy, s.modified_by AS ModifiedBy
                FROM sales s
                WHERE s.id = @Id";
            
            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var sale = await conn.QueryFirstOrDefaultAsync<Sale>(query, new { Id = id });
                
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
                INSERT INTO sales
                    (client_id, sale_date, total_amount, nit, created_at, last_modification, is_active, created_by)
                VALUES
                    (@ClientId, @SaleDate, @TotalAmount, @Nit, @CreatedAt, @LastModification, @IsActive, @CreatedBy)
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
                UPDATE sales
                   SET client_id = @ClientId,
                       sale_date = @SaleDate,
                       total_amount = @TotalAmount,
                       nit = @Nit,
                       last_modification = @LastModification,
                       modified_by = @ModifiedBy
                 WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                sale.ModifiedBy = userEmail;

                var affected = await conn.ExecuteAsync(query, sale);
                
                return affected == 0
                    ? Result<Sale>.Failure("No se encontro la venta para actualizar.")
                    : Result<Sale>.Success(sale);
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
                UPDATE sales
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

        public async Task<Result> UpdateSaleStatus(int id, string status)
        {
            const string query = @"
                UPDATE sales
                SET status = @Status,
                    last_modification = @LastModification
                WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var affected = await conn.ExecuteAsync(query, new
                {
                    Id = id,
                    Status = status,
                    LastModification = DateTime.UtcNow
                });

                return affected > 0 ? Result.Success() : Result.Failure("No se encontro la venta para actualizar el estado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en UpdateStatus");
                return Result.Failure("Error al actualizar el estado de la venta.");
            }
        }
    }
}