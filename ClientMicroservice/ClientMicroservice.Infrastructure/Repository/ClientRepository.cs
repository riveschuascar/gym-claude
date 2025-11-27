using ClientMicroservice.Domain.Entities;
using ClientMicroservice.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ClientMicroservice.Infrastructure.Repository;

public class ClientRepository : IClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PostgresGymDB")
            ?? throw new InvalidOperationException("Connection string 'PostgresGymDB' not found.");
    }

    public async Task<Client> CreateAsync(Client client, string? userEmail = null)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var tx = await conn.BeginTransactionAsync();
        try
        {
            const string clientSql = @"
                INSERT INTO public.client (name, first_lastname, second_lastname, date_birth, ci, is_active, created_at,
                                           fitness_level, initial_weight_kg, current_weight_kg, emergency_contact_phone,
                                           created_by)
                VALUES (@name, @first_lastname, @second_lastname, @date_birth, @ci, @is_active, @created_at,
                        @fitness_level, @initial_weight_kg, @current_weight_kg, @emergency_contact_phone,
                        @created_by)
                RETURNING id;";

            await using (var cmd = new NpgsqlCommand(clientSql, conn, tx))
            {
                cmd.Parameters.AddWithValue("name", (object?)client.Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("first_lastname", (object?)client.FirstLastname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("second_lastname", (object?)client.SecondLastname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("date_birth", (object?)client.DateBirth ?? DBNull.Value);
                cmd.Parameters.AddWithValue("ci", (object?)client.Ci ?? DBNull.Value);
                cmd.Parameters.AddWithValue("is_active", client.IsActive);
                cmd.Parameters.AddWithValue("created_at", (object?)client.CreatedAt ?? DateTime.UtcNow);
                cmd.Parameters.AddWithValue("fitness_level", (object?)client.FitnessLevel ?? DBNull.Value);
                cmd.Parameters.AddWithValue("initial_weight_kg", (object?)client.InitialWeightKg ?? DBNull.Value);
                cmd.Parameters.AddWithValue("current_weight_kg", (object?)client.CurrentWeightKg ?? DBNull.Value);
                cmd.Parameters.AddWithValue("emergency_contact_phone", (object?)client.EmergencyContactPhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("created_by", (object?)userEmail ?? DBNull.Value);
                
                var id = (int)(await cmd.ExecuteScalarAsync()!);
                client.Id = id;
            }

            await tx.CommitAsync();
            return client;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        const string sql = @"
            SELECT id, created_at, last_modification, is_active, name, first_lastname, second_lastname, date_birth, ci,
                   fitness_level, initial_weight_kg, current_weight_kg, emergency_contact_phone
            FROM public.client
            ORDER BY id DESC;";

        var list = new List<Client>();
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapReader(reader));
        }
        return list;
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT id, created_at, last_modification, is_active, name, first_lastname, second_lastname, date_birth, ci,
                   fitness_level, initial_weight_kg, current_weight_kg, emergency_contact_phone
            FROM public.client
            WHERE id = @id;";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapReader(reader);
        }
        return null;
    }

    public async Task<Client?> UpdateAsync(Client client, string? userEmail = null)
    {
        client.LastModification = DateTime.UtcNow;
        const string sql = @"
            UPDATE public.client
               SET name = @name,
                   first_lastname = @first_lastname,
                   second_lastname = @second_lastname,
                   date_birth = @date_birth,
                   ci = @ci,
                   last_modification = @last_modification,
                   fitness_level = @fitness_level,
                   current_weight_kg = @current_weight_kg,
                   emergency_contact_phone = @emergency_contact_phone,
                   modified_by = @modified_by
             WHERE id = @id;";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", client.Id);
        cmd.Parameters.AddWithValue("name", (object?)client.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("first_lastname", (object?)client.FirstLastname ?? DBNull.Value);
        cmd.Parameters.AddWithValue("second_lastname", (object?)client.SecondLastname ?? DBNull.Value);
        cmd.Parameters.AddWithValue("date_birth", (object?)client.DateBirth ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ci", (object?)client.Ci ?? DBNull.Value);
        cmd.Parameters.AddWithValue("last_modification", (object?)client.LastModification ?? DBNull.Value);
        cmd.Parameters.AddWithValue("fitness_level", (object?)client.FitnessLevel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("current_weight_kg", (object?)client.CurrentWeightKg ?? DBNull.Value);
        cmd.Parameters.AddWithValue("emergency_contact_phone", (object?)client.EmergencyContactPhone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modified_by", (object?)userEmail ?? DBNull.Value);
        
        var affected = await cmd.ExecuteNonQueryAsync();
        return affected >= 1 ? client : null;
    }

    public async Task<bool> DeleteByIdAsync(int id, string? userEmail = null)
    {
        const string sql = @"
            DELETE FROM public.client
            WHERE id = @id;";
            
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        var affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static Client MapReader(NpgsqlDataReader reader)
    {
        return new Client
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
            LastModification = reader.IsDBNull(reader.GetOrdinal("last_modification")) ? null : reader.GetDateTime(reader.GetOrdinal("last_modification")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString(reader.GetOrdinal("name")),
            FirstLastname = reader.IsDBNull(reader.GetOrdinal("first_lastname")) ? null : reader.GetString(reader.GetOrdinal("first_lastname")),
            SecondLastname = reader.IsDBNull(reader.GetOrdinal("second_lastname")) ? null : reader.GetString(reader.GetOrdinal("second_lastname")),
            DateBirth = reader.IsDBNull(reader.GetOrdinal("date_birth")) ? null : reader.GetDateTime(reader.GetOrdinal("date_birth")),
            Ci = reader.IsDBNull(reader.GetOrdinal("ci")) ? null : reader.GetString(reader.GetOrdinal("ci")),
            FitnessLevel = reader.IsDBNull(reader.GetOrdinal("fitness_level")) ? null : reader.GetString(reader.GetOrdinal("fitness_level")),
            InitialWeightKg = reader.IsDBNull(reader.GetOrdinal("initial_weight_kg")) ? null : reader.GetDecimal(reader.GetOrdinal("initial_weight_kg")),
            CurrentWeightKg = reader.IsDBNull(reader.GetOrdinal("current_weight_kg")) ? null : reader.GetDecimal(reader.GetOrdinal("current_weight_kg")),
            EmergencyContactPhone = reader.IsDBNull(reader.GetOrdinal("emergency_contact_phone")) ? null : reader.GetString(reader.GetOrdinal("emergency_contact_phone"))
        };
    }
}