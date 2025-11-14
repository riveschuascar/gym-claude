using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Ports;
using UserMicroservice.Domain.Shared;
using Dapper;
using System.Data;

namespace UserMicroservice.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<Result<IEnumerable<User>>> GetAll()
        {
            const string query = @"SELECT * FROM users.user WHERE IsActive = true;";

            try
            {
                var users = await _connection.QueryAsync<User>(query);
                return Result<IEnumerable<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<User>>.Failure($"Error al obtener usuarios: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetById(int id)
        {
            const string query = @"SELECT * FROM users.user WHERE IsActive = true AND Id = @Id;";

            try
            {
                var user = await _connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
                return user is null
                    ? Result<User>.Failure("Usuario no encontrado.")
                    : Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error al obtener usuario: {ex.Message}");
            }
        }

        public async Task<Result<User>> Create(User entity)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.LastModification = DateTime.UtcNow;
                entity.IsActive = true;

                const string query = @"
                INSERT INTO users.user
                    (Name, FirstLastname, SecondLastname, DateOfBirth, Ci, UserRole, HireDate, MonthlySalary, Specialization, Email, Password, MustChangePassword, CreatedAt, LastModification, IsActive)
                VALUES 
                    (@Name, @FirstLastname, @SecondLastname, @DateOfBirth, @Ci, @UserRole, @HireDate, @MonthlySalary, @Specialization, @Email, crypt(@Password, gen_salt('bf')), @MustChangePassword, @CreatedAt, @LastModification, @IsActive)
                RETURNING Id;";

                entity.Id = await _connection.ExecuteScalarAsync<int>(query, entity, transaction);
                transaction.Commit();

                return Result<User>.Success(entity);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<User>.Failure($"Error al crear usuario: {ex.Message}");
            }
        }

        public async Task<Result<User>> Update(User entity)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                if (string.IsNullOrWhiteSpace(entity.UserRole))
                {
                    const string existingRoleQuery = @"SELECT UserRole FROM users.user WHERE Id = @Id;";
                    var existingRole = await _connection.ExecuteScalarAsync<string>(existingRoleQuery, new { Id = entity.Id }, transaction);
                    entity.UserRole = existingRole;
                }

                entity.LastModification = DateTime.UtcNow;

                const string updateQuery = @"
                UPDATE users.user
                SET Name = @Name,
                    FirstLastname = @FirstLastname,
                    SecondLastname = @SecondLastname,
                    DateOfBirth = @DateOfBirth,
                    Ci = @Ci,
                    UserRole = @UserRole,
                    HireDate = @HireDate,
                    MonthlySalary = @MonthlySalary,
                    Specialization = @Specialization,
                    Email = @Email,
                    LastModification = @LastModification,
                    IsActive = @IsActive
                WHERE Id = @Id;";

                var affected = await _connection.ExecuteAsync(updateQuery, entity, transaction);

                if (affected == 0)
                {
                    transaction.Rollback();
                    return Result<User>.Failure("No se encontró el usuario para actualizar.");
                }

                transaction.Commit();
                return Result<User>.Success(entity);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<User>.Failure($"Error al actualizar usuario: {ex.Message}");
            }
        }

        public async Task<Result> DeleteById(int id)
        {
            const string query = @"UPDATE users.user SET IsActive = false, LastModification = @LastModification WHERE Id = @Id;";

            try
            {
                var affected = await _connection.ExecuteAsync(query, new { Id = id, LastModification = DateTime.UtcNow });
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
            const string query = @"SELECT * FROM users.user WHERE IsActive = true AND Email = @Email";

            try
            {
                var user = await _connection.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
                return user is null
                    ? Result<User>.Failure("Usuario no encontrado con el correo electrónico.")
                    : Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error al buscar usuario con email: {ex.Message}");
            }
        }

        public async Task<Result> UpdatePassword(string email, string password)
        {
            const string query = @"UPDATE users.user SET Password = @Password, MustChangePassword = false WHERE Id = @Id";

            try
            {
                var affected = await _connection.ExecuteAsync(query, new { Email = email, Password = password });
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
