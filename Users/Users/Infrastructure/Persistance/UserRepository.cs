using Domain.Entities;
using Domain.Ports;
using Domain.Shared;
using Dapper;
using Npgsql;

namespace Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IUserConnectionProvider connectionProvider)
        {
            ArgumentNullException.ThrowIfNull(connectionProvider, nameof(connectionProvider));

            _connectionString = connectionProvider.GetConnectionString()
                                ?? throw new InvalidOperationException("El connection provider debe entregar una cadena válida.");
        }

        public async Task<Result<IEnumerable<User>>> GetAll()
        {
            using var conn = new NpgsqlConnection(_connectionString);

            const string query = @"SELECT * FROM users.user WHERE Id = @Id;";

            try
            {
                var users = await conn.QueryAsync<User>(query, new { Id = DBNull.Value });
                return Result<IEnumerable<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<User>>.Failure($"Error al obtener usuarios: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            const string query = @"SELECT * FROM users.user WHERE IsActive = true;";

            try
            {
                var user = await conn.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
                if (user is null)
                    return Result<User>.Failure("Usuario no encontrado.");

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error al obtener usuario: {ex.Message}");
            }
        }

        public async Task<Result<User>> Create(User entity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.LastModification = DateTime.UtcNow;
                entity.IsActive = true;

                const string query = @"
                INSERT INTO ""User"" 
                    (Name, FirstLastname, SecondLastname, DateOfBirth, Ci, Role, HireDate, MonthlySalary, Specialization, Email, Password, MustChangePassword, CreatedAt, LastModification, IsActive)
                VALUES 
                    (@Name, @FirstLastname, @SecondLastname, @DateOfBirth, @Ci, @Role, @HireDate, @MonthlySalary, @Specialization, @Email, @Password, @MustChangePassword, @CreatedAt, @LastModification, @IsActive)
                RETURNING Id;";

                entity.Id = await conn.ExecuteScalarAsync<int>(query, entity, transaction);

                await transaction.CommitAsync();
                return Result<User>.Success(entity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<User>.Failure($"Error al crear usuario: {ex.Message}");
            }
        }

        public async Task<Result<User>> Update(User entity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrWhiteSpace(entity.Role))
                {
                    const string existingRoleQuery = @"SELECT Role FROM users.user WHERE Id = @Id;";
                    var existingRole = await conn.ExecuteScalarAsync<string>(existingRoleQuery, new { Id = entity.Id }, transaction);
                    entity.Role = existingRole;
                }

                entity.LastModification = DateTime.UtcNow;

                const string updateQuery = @"
                UPDATE users.user
                SET Name = @Name,
                    FirstLastname = @FirstLastname,
                    SecondLastname = @SecondLastname,
                    DateOfBirth = @DateOfBirth,
                    Ci = @Ci,
                    Role = @Role,
                    HireDate = @HireDate,
                    MonthlySalary = @MonthlySalary,
                    Specialization = @Specialization,
                    Email = @Email,
                    Password = @Password,
                    MustChangePassword = @MustChangePassword,
                    LastModification = @LastModification,
                    IsActive = @IsActive
                WHERE Id = @Id;";

                var affected = await conn.ExecuteAsync(updateQuery, entity, transaction);

                if (affected == 0)
                {
                    await transaction.RollbackAsync();
                    return Result<User>.Failure("No se encontró el usuario para actualizar.");
                }

                await transaction.CommitAsync();
                return Result<User>.Success(entity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<User>.Failure($"Error al actualizar usuario: {ex.Message}");
            }
        }

        public async Task<Result> DeleteById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            const string query = @"UPDATE users.user SET IsActive = false, LastModification = @LastModification WHERE Id = @Id;";

            try
            {
                var affected = await conn.ExecuteAsync(query, new { Id = id, LastModification = DateTime.UtcNow });
                return affected > 0
                    ? Result.Success()
                    : Result.Failure("No se encontró el usuario a eliminar.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al eliminar usuario: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetByEmail(string email)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            const string query = @"SELEC * FROM users.user WHERE IsActive = true AND Email = @Email";

            try
            {
                var user = await conn.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
                return user is null
                    ? Result<User>.Failure("Usuario no encontrado con el correo electrónico.")
                    : Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error al buscar usuario con email: {ex.Message}");
            }
        }

        public async Task<Result> UpdatePassword(int id, string password)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            const string query = @"UPDATE users.user SET Password = @Password, MustChangePassword = false WHERE Id = @Id";

            try
            {
                var affected = await conn.ExecuteAsync(query, new { Id = id, Password = password });
                return affected > 0
                    ? Result.Success()
                    : Result.Failure("No se encontró el usuario para actualizar contraseña.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al actualizar contraseña: {ex.Message}");
            }
        }
    }
}
