using Dapper;
using Npgsql;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Ports;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DisciplineMicroservice.DisciplineMicroserviceInfraestructure.Persistance
{
    public class DisciplineRepository : IDisciplineRepository
    {
        private readonly string _connectionString;

        public DisciplineRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DisciplineMicroserviceDB")
                ?? throw new Exception("No se encontró la cadena de conexión 'DisciplineMicroserviceDB'");
            Console.WriteLine("ConnectionString: " + _connectionString);
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Obtener todas las disciplinas activas
        public async Task<Result<IEnumerable<Discipline>>> GetAll()
        {
            const string query = @"
            SELECT id AS Id,
                   name AS Name,
                   start_time AS StartTime,
                   end_time AS EndTime,
                   created_at AS CreatedAt,
                   last_modification AS LastModification,
                   is_active AS IsActive
            FROM discipline
            WHERE is_active = true;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var data = await conn.QueryAsync<Discipline>(query);
                return Result<IEnumerable<Discipline>>.Success(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAll: " + ex.Message);
                return Result<IEnumerable<Discipline>>.Failure("Error al obtener disciplinas.");
            }
        }

        // Obtener disciplina por Id
        public async Task<Result<Discipline>> GetById(int id)
        {
            const string query = @"
            SELECT id AS Id,
                   name AS Name,
                   start_time AS StartTime,
                   end_time AS EndTime,
                   created_at AS CreatedAt,
                   last_modification AS LastModification,
                   is_active AS IsActive
            FROM discipline
            WHERE id = @Id AND is_active = true;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();
                var discipline = await conn.QuerySingleOrDefaultAsync<Discipline>(query, new { Id = id });

                if (discipline == null)
                    return Result<Discipline>.Failure($"No se encontró la disciplina con Id {id}");

                return Result<Discipline>.Success(discipline);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetById: " + ex.Message);
                return Result<Discipline>.Failure("Error al obtener disciplina.");
            }
        }

        // Crear nueva disciplina
        public async Task<Result<Discipline>> Create(Discipline entity)
        {
            const string query = @"
            INSERT INTO discipline
                (name, start_time, end_time, created_at, last_modification, is_active)
            VALUES
                (@Name, @StartTime, @EndTime, @CreatedAt, @LastModification, @IsActive)
            RETURNING id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                entity.CreatedAt = DateTime.UtcNow;
                entity.LastModification = DateTime.UtcNow;
                entity.IsActive = true;

                var id = await conn.ExecuteScalarAsync<int>(query, entity);
                entity.Id = (short)id;

                return Result<Discipline>.Success(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Create: " + ex.Message);
                return Result<Discipline>.Failure("Error al crear disciplina.");
            }
        }

        // Actualizar disciplina
        public async Task<Result<Discipline>> Update(Discipline entity)
        {
            const string query = @"
            UPDATE discipline
            SET name = @Name,
                start_time = @StartTime,
                end_time = @EndTime,
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
                    return Result<Discipline>.Failure("No se encontró la disciplina para actualizar.");

                return Result<Discipline>.Success(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en Update: " + ex.Message);
                return Result<Discipline>.Failure("Error al actualizar disciplina.");
            }
        }

        // Eliminar disciplina (marcar como inactiva)
        public async Task<Result> DeleteById(int id)
        {
            const string query = @"
            UPDATE discipline
            SET is_active = false,
                last_modification = @LastModification
            WHERE id = @Id;";

            try
            {
                using var conn = CreateConnection();
                await conn.OpenAsync();

                var affected = await conn.ExecuteAsync(query, new { Id = id, LastModification = DateTime.UtcNow });

                return affected > 0 ? Result.Success() : Result.Failure("No se encontró la disciplina para eliminar.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteById: " + ex.Message);
                return Result.Failure("Error al eliminar disciplina.");
            }
        }
    }
}
