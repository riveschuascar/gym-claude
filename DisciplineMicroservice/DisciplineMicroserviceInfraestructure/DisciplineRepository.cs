using Dapper;
using System.Data;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Ports;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;

namespace DisciplineMicroservice.DisciplineMicroserviceInfraestructure
{
    public class DisciplineRepository : IDisciplineRepository
    {
        private readonly IDbConnection _connection;

        public DisciplineRepository(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<Result<IEnumerable<Discipline>>> GetAll()
        {
            const string query = @"SELECT * FROM Discipline WHERE IsActive = 1;";

            try
            {
                var disciplines = await _connection.QueryAsync<Discipline>(query);
                return Result<IEnumerable<Discipline>>.Success(disciplines);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Discipline>>.Failure($"Error al obtener disciplinas: {ex.Message}");
            }
        }

        public async Task<Result<Discipline>> GetById(int id)
        {
            const string query = @"SELECT * FROM Discipline WHERE DisciplineId = @Id AND IsActive = 1;";

            try
            {
                var discipline = await _connection.QuerySingleOrDefaultAsync<Discipline>(query, new { Id = id });

                return discipline is null
                    ? Result<Discipline>.Failure("Disciplina no encontrada.")
                    : Result<Discipline>.Success(discipline);
            }
            catch (Exception ex)
            {
                return Result<Discipline>.Failure($"Error al obtener disciplina: {ex.Message}");
            }
        }

        public async Task<Result<Discipline>> Create(Discipline entity)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.LastModification = DateTime.UtcNow;
                entity.IsActive = true;

                const string query = @"
                INSERT INTO Discipline
                    (Name, StartTime, EndTime, CreatedAt, LastModification, IsActive)
                VALUES 
                    (@Name, @StartTime, @EndTime, @CreatedAt, @LastModification, @IsActive);
                
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int id = await _connection.ExecuteScalarAsync<int>(query, entity, transaction);
                entity.Id = (short)id;

                transaction.Commit();

                return Result<Discipline>.Success(entity);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<Discipline>.Failure($"Error al crear disciplina: {ex.Message}");
            }
        }

        public async Task<Result<Discipline>> Update(Discipline entity)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                entity.LastModification = DateTime.UtcNow;

                const string query = @"
                UPDATE Discipline
                SET 
                    Name = @Name,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    LastModification = @LastModification,
                    IsActive = @IsActive
                WHERE DisciplineId = @Id;";

                var affected = await _connection.ExecuteAsync(query, entity, transaction);

                if (affected == 0)
                {
                    transaction.Rollback();
                    return Result<Discipline>.Failure("No se encontró la disciplina para actualizar.");
                }

                transaction.Commit();
                return Result<Discipline>.Success(entity);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<Discipline>.Failure($"Error al actualizar disciplina: {ex.Message}");
            }
        }

        public async Task<Result> DeleteById(int id)
        {
            const string query = @"UPDATE Discipline 
                                   SET IsActive = 0, LastModification = @LastModification 
                                   WHERE DisciplineId = @Id;";

            try
            {
                var affected = await _connection.ExecuteAsync(query, new { Id = id, LastModification = DateTime.UtcNow });

                return affected > 0
                    ? Result.Success()
                    : Result.Failure("No se encontró la disciplina para eliminar.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al eliminar disciplina: {ex.Message}");
            }
        }
    }
}
