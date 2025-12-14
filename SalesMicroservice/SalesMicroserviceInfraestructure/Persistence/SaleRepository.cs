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
                       s.is_active AS IsActive, s.created_by AS CreatedBy, s.modified_by AS ModifiedBy,
                       
                       sd.id,
                       sd.sale_id AS SaleId, sd.discipline_id AS DisciplineId,
                       sd.qty AS Qty, sd.price AS Price, sd.total AS Total, sd.start_date AS StartDate, sd.end_date AS EndDate
                  FROM sales s
             LEFT JOIN sale_details sd ON sd.sale_id = s.id
                 WHERE s.is_active = true
                 ORDER BY s.created_at DESC;";
            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var saleDict = new Dictionary<int, Sale>();
                var data = await conn.QueryAsync<Sale, SaleDetail, Sale>(query,
                    (s, d) =>
                    {
                        if (!saleDict.TryGetValue(s.Id ?? 0, out var sale))
                        {
                            sale = s;
                            sale.Details = new List<SaleDetail>();
                            saleDict.Add(s.Id ?? 0, sale);
                        }

                        if (d != null && d.Id.HasValue)
                            sale.Details.Add(d);

                        return sale;
                    }, splitOn: "id"); 

                return Result<IEnumerable<Sale>>.Success(saleDict.Values);
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
                SELECT s.id, s.client_id AS ClientId, s.sale_date AS SaleDate,
                       s.total_amount AS TotalAmount, s.nit AS Nit,
                       s.created_at AS CreatedAt, s.last_modification AS LastModification,
                       s.is_active AS IsActive, s.created_by AS CreatedBy, s.modified_by AS ModifiedBy,
                       
                       sd.id, 
                       sd.sale_id AS SaleId, sd.discipline_id AS DisciplineId,
                       sd.qty AS Qty, sd.price AS Price, sd.total AS Total, sd.start_date AS StartDate, sd.end_date AS EndDate
                  FROM sales s
             LEFT JOIN sale_details sd ON sd.sale_id = s.id
                 WHERE s.id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var saleDict = new Dictionary<int, Sale>();
                await conn.QueryAsync<Sale, SaleDetail, Sale>(query,
                    (s, d) =>
                    {
                        if (!saleDict.TryGetValue(s.Id ?? 0, out var sale))
                        {
                            sale = s;
                            sale.Details = new List<SaleDetail>();
                            saleDict.Add(s.Id ?? 0, sale);
                        }
                        if (d != null && d.Id.HasValue)
                            sale.Details.Add(d);
                        return sale;
                    }, new { Id = id }, splitOn: "id"); 
                var sale = saleDict.Values.FirstOrDefault();
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

                using var tran = conn.BeginTransaction();
                var id = await conn.ExecuteScalarAsync<int>(query, sale, tran);
                sale.Id = id;

                // insert sale details
                if (sale.Details != null && sale.Details.Any())
                {
                    foreach (var d in sale.Details)
                    {
                        d.SaleId = id;
                        const string detailQuery = @"INSERT INTO sale_details (sale_id, discipline_id, qty, price, total, start_date, end_date)
                                                      VALUES (@SaleId, @DisciplineId, @Qty, @Price, @Total, @StartDate, @EndDate);";
                            await conn.ExecuteAsync(detailQuery, d, tran);
                    }
                }

                await tran.CommitAsync();
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

                using var tran = conn.BeginTransaction();
                var affected = await conn.ExecuteAsync(query, sale, tran);
                if (affected == 0)
                    return Result<Sale>.Failure("No se encontro la venta para actualizar.");
                // replace details: delete existing & insert new
                const string deleteDetails = "DELETE FROM sale_details WHERE sale_id = @SaleId;";
                await conn.ExecuteAsync(deleteDetails, new { SaleId = sale.Id }, tran);
                if (sale.Details != null && sale.Details.Any())
                {
                    foreach (var d in sale.Details)
                    {
                        d.SaleId = sale.Id ?? 0;
                        const string detailQuery = @"INSERT INTO sale_details (sale_id, discipline_id, qty, price, total, start_date, end_date)
                                                      VALUES (@SaleId, @DisciplineId, @Qty, @Price, @Total, @StartDate, @EndDate);";
                        await conn.ExecuteAsync(detailQuery, d, tran);
                    }
                }
                await tran.CommitAsync();
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
    }
}