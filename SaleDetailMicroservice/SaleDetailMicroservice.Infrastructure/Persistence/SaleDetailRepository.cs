using Dapper;
using Npgsql;
using SaleDetailMicroservice.Domain.Entities;
using SaleDetailMicroservice.Domain.Ports;
using SaleDetailMicroservice.Domain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaleDetailMicroservice.Infrastructure.Persistence
{
    public class SaleDetailRepository : ISaleDetailRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SaleDetailRepository> _logger;

        public SaleDetailRepository(IConfiguration configuration, ILogger<SaleDetailRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("SaleDetailMicroserviceDB")
                ?? throw new Exception("Cadena de conexión 'SaleDetailMicroserviceDB' no encontrada.");
            _logger = logger;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<Result<IEnumerable<SaleDetail>>> GetAll()
        {
            const string query = @"
                SELECT id, sale_id AS SaleId, discipline_id AS DisciplineId,
                       qty, price, total, start_date AS StartDate, end_date AS EndDate
                FROM sale_details
                ORDER BY id DESC;";

            try
            {
                using var conn = CreateConnection();
                var list = await conn.QueryAsync<SaleDetail>(query);
                return Result<IEnumerable<SaleDetail>>.Success(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll SaleDetails");
                return Result<IEnumerable<SaleDetail>>.Failure("Error al obtener los detalles.");
            }
        }

        public async Task<Result<IEnumerable<SaleDetail>>> GetBySaleId(int saleId)
        {
            const string query = @"
                SELECT id, sale_id AS SaleId, discipline_id AS DisciplineId,
                       qty, price, total, start_date AS StartDate, end_date AS EndDate
                FROM sale_details
                WHERE sale_id = @SaleId;";

            try
            {
                using var conn = CreateConnection();
                var list = await conn.QueryAsync<SaleDetail>(query, new { SaleId = saleId });
                return Result<IEnumerable<SaleDetail>>.Success(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetBySaleId SaleDetails");
                return Result<IEnumerable<SaleDetail>>.Failure("Error al obtener los detalles de la venta.");
            }
        }

        public async Task<Result> CreateRange(IEnumerable<SaleDetail> details)
        {
            const string query = @"
                INSERT INTO sale_details (sale_id, discipline_id, qty, price, total, start_date, end_date)
                VALUES (@SaleId, @DisciplineId, @Qty, @Price, @Total, @StartDate, @EndDate);";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                using var tran = conn.BeginTransaction();

                // Dapper ejecuta la query para cada elemento de la lista automáticamente
                await conn.ExecuteAsync(query, details, tran);

                await tran.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CreateRange SaleDetails");
                return Result.Failure("Error al insertar los detalles.");
            }
        }
    }
}