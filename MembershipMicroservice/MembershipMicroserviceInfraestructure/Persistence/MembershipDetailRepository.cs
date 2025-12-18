using Dapper;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Ports;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MembershipMicroservice.MembershipMicroserviceInfraestructure.Persistence
{
    public class MembershipDetailRepository : IMembershipDetailRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MembershipDetailRepository> _logger;

        public MembershipDetailRepository(IConfiguration configuration, ILogger<MembershipDetailRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("MembershipMicroserviceDB")
                ?? throw new Exception("No se encontro la cadena de conexion 'MembershipMicroserviceDB'");
            _logger = logger;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<Result<IEnumerable<MembershipDiscipline>>> GetByMembershipId(int membershipId)
        {
            const string query = @"
                SELECT id AS Id,
                       membership_id AS MembershipId,
                       discipline_id AS DisciplineId,
                       created_at AS CreatedAt,
                       last_modification AS LastModification,
                       is_active AS IsActive,
                       created_by AS CreatedBy,
                       modified_by AS ModifiedBy
                  FROM membership_discipline
                 WHERE membership_id = @MembershipId
                   AND is_active = true
                 ORDER BY id DESC;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var data = await conn.QueryAsync<MembershipDiscipline>(query, new { MembershipId = membershipId });
                return Result<IEnumerable<MembershipDiscipline>>.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetByMembershipId");
                return Result<IEnumerable<MembershipDiscipline>>.Failure("Error al obtener disciplinas de la membresia.");
            }
        }

        public async Task<Result<MembershipDiscipline>> Create(MembershipDiscipline entity, string? userEmail = null)
        {
            const string existsQuery = @"
                SELECT 1
                  FROM membership_discipline
                 WHERE membership_id = @MembershipId
                   AND discipline_id = @DisciplineId
                   AND is_active = true
                 LIMIT 1;";

            const string insertQuery = @"
                INSERT INTO membership_discipline
                    (membership_id, discipline_id, created_at, last_modification, is_active, created_by)
                VALUES
                    (@MembershipId, @DisciplineId, @CreatedAt, @LastModification, @IsActive, @CreatedBy)
                RETURNING id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var alreadyExists = await conn.ExecuteScalarAsync<bool>(existsQuery, new
                {
                    entity.MembershipId,
                    entity.DisciplineId
                });

                if (alreadyExists)
                    return Result<MembershipDiscipline>.Failure("La disciplina ya esta asignada a la membresia.");

                entity.CreatedAt = DateTime.UtcNow;
                entity.LastModification = DateTime.UtcNow;
                entity.IsActive = true;
                entity.CreatedBy = userEmail;

                var id = await conn.ExecuteScalarAsync<int>(insertQuery, new
                {
                    entity.MembershipId,
                    entity.DisciplineId,
                    entity.CreatedAt,
                    entity.LastModification,
                    entity.IsActive,
                    CreatedBy = userEmail
                });

                entity.Id = id;

                return Result<MembershipDiscipline>.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Create");
                return Result<MembershipDiscipline>.Failure("Error al agregar la disciplina a la membresia.");
            }
        }

        public async Task<Result> Delete(int membershipId, int disciplineId, string? userEmail = null)
        {
            const string query = @"
                UPDATE membership_discipline
                   SET is_active = false,
                       last_modification = @LastModification,
                       modified_by = @ModifiedBy
                 WHERE membership_id = @MembershipId
                   AND discipline_id = @DisciplineId;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var affected = await conn.ExecuteAsync(query, new
                {
                    MembershipId = membershipId,
                    DisciplineId = disciplineId,
                    LastModification = DateTime.UtcNow,
                    ModifiedBy = userEmail
                });

                return affected > 0
                    ? Result.Success()
                    : Result.Failure("No se encontro la asignacion a eliminar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Delete");
                return Result.Failure("Error al eliminar la disciplina de la membresia.");
            }
        }
    }
}
