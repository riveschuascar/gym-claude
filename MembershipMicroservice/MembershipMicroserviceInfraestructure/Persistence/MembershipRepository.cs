using Dapper;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Ports;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MembershipMicroservice.MembershipMicroserviceInfraestructure.Persistence
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MembershipRepository> _logger;

        public MembershipRepository(IConfiguration config, ILogger<MembershipRepository> logger)
        {
            _connectionString = config.GetConnectionString("MembershipMicroserviceDB")
                ?? throw new Exception("No se encontró la cadena de conexión 'MembershipMicroserviceDB'");
            _logger = logger;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<Result<IEnumerable<Membership>>> GetAll()
        {
            const string query = @"
                SELECT id AS Id,
                       name AS Name,
                       price AS Price,
                       description AS Description,
                       monthly_sessions AS MonthlySessions,
                       created_at AS CreatedAt,
                       last_modification AS LastModification,
                       is_active AS IsActive
                FROM membership
                WHERE is_active = true
                ORDER BY name ASC;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var data = await conn.QueryAsync<Membership>(query);
                return Result<IEnumerable<Membership>>.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll");
                return Result<IEnumerable<Membership>>.Failure("Error al obtener membresías.");
            }
        }

        public async Task<Result<Membership>> GetById(int id)
        {
            if (id <= 0)
                return Result<Membership>.Failure("El Id debe ser mayor a cero.");

            const string query = @"
                SELECT id AS Id,
                       name AS Name,
                       price AS Price,
                       description AS Description,
                       monthly_sessions AS MonthlySessions,
                       created_at AS CreatedAt,
                       last_modification AS LastModification,
                       is_active AS IsActive
                FROM membership
                WHERE id = @Id AND is_active = true;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var membership = await conn.QuerySingleOrDefaultAsync<Membership>(query, new { Id = id });
                if (membership == null)
                    return Result<Membership>.Failure($"No se encontró la membresía con Id {id}");

                return Result<Membership>.Success(membership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetById");
                return Result<Membership>.Failure("Error al obtener la membresía.");
            }
        }

        public async Task<Result<Membership>> Create(Membership entity)
        {
            const string query = @"
                INSERT INTO membership
                    (name, price, description, monthly_sessions, created_at, last_modification, is_active)
                VALUES
                    (@Name, @Price, @Description, @MonthlySessions, @CreatedAt, @LastModification, @IsActive)
                RETURNING id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                entity.CreatedAt = DateTime.UtcNow;
                entity.LastModification = DateTime.UtcNow;
                entity.IsActive = true;

                var id = await conn.ExecuteScalarAsync<int>(query, entity);
                entity.Id = id;

                return Result<Membership>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Create");
                return Result<Membership>.Failure("Error al crear la membresía.");
            }
        }

        public async Task<Result<Membership>> Update(Membership entity)
        {
            const string query = @"
                UPDATE membership
                SET name = @Name,
                    price = @Price,
                    description = @Description,
                    monthly_sessions = @MonthlySessions,
                    last_modification = @LastModification,
                    is_active = @IsActive
                WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                entity.LastModification = DateTime.UtcNow;
                var affected = await conn.ExecuteAsync(query, entity);

                if (affected == 0)
                    return Result<Membership>.Failure("No se encontró la membresía para actualizar.");

                return Result<Membership>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Update");
                return Result<Membership>.Failure("Error al actualizar membresía.");
            }
        }

        public async Task<Result> DeleteById(int id)
        {
            if (id <= 0)
                return Result.Failure("El Id debe ser mayor a cero.");

            const string query = @"
                UPDATE membership
                SET is_active = false,
                    last_modification = @LastModification
                WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var affected = await conn.ExecuteAsync(query, new { Id = id, LastModification = DateTime.UtcNow });
                return affected > 0 ? Result.Success() : Result.Failure("No se encontró la membresía para eliminar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DeleteById");
                return Result.Failure("Error al eliminar membresía.");
            }
        }
        public async Task<Result<bool>> UpdateMembershipDisciplines(int membershipId, IEnumerable<int> disciplineIds)
        {
            const string deleteQuery = @"
        DELETE FROM MembershipDisciplines
        WHERE MembershipId = @MembershipId;";

            const string insertQuery = @"
        INSERT INTO MembershipDisciplines (MembershipId, DisciplineId)
        VALUES (@MembershipId, @DisciplineId);";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                using var transaction = await conn.BeginTransactionAsync();

                // Borramos los existentes
                await conn.ExecuteAsync(deleteQuery, new { MembershipId = membershipId }, transaction);

                // Insertamos los nuevos
                foreach (var disciplineId in disciplineIds)
                {
                    await conn.ExecuteAsync(insertQuery, new { MembershipId = membershipId, DisciplineId = disciplineId }, transaction);
                }

                await transaction.CommitAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar MembershipDisciplines");
                return Result<bool>.Failure("Error al actualizar las disciplinas de la membresía.");
            }
        }

        public async Task<Result<IEnumerable<int>>> GetMembershipDisciplineIds(int membershipId)
        {
            const string query = @"
        SELECT DisciplineId
        FROM MembershipDisciplines
        WHERE MembershipId = @MembershipId;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var ids = await conn.QueryAsync<int>(query, new { MembershipId = membershipId });
                return Result<IEnumerable<int>>.Success(ids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener MembershipDisciplines");
                return Result<IEnumerable<int>>.Failure("Error al obtener las disciplinas de la membresía.");
            }
        }

    }
}