using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Ports;
using UserMicroservice.Domain.Shared;
using Dapper;
using System.Data;

namespace UserMicroservice.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _conn;

        public UserRepository(IDbConnection conn)
        {
            _conn = conn ?? throw new ArgumentNullException(nameof(conn));
            if (_conn.State == ConnectionState.Closed)
                _conn.Open();
        }

        public async Task<Result<IEnumerable<User>>> GetAll()
        {
            const string query = @"SELECT * FROM users.user WHERE IsActive = true ORDER BY FirstLastname, SecondLastname;";

            var users = await _conn.QueryAsync<User>(query);

            return users is null
                ? Result<IEnumerable<User>>.Failure("Error al obtener usuarios")
                : Result<IEnumerable<User>>.Success(users);
        }

        public async Task<Result<User>> GetById(int id)
        {
            const string query = @"SELECT * FROM users.user WHERE IsActive = true AND Id = @Id ORDER BY FirstLastname, SecondLastname;";

            var user = await _conn.QuerySingleOrDefaultAsync<User>(query, new { Id = id });

            return user is null
                ? Result<User>.Failure("Usuario no encontrado.")
                : Result<User>.Success(user);
        }

        public async Task<Result> Create(User entity)
        {
            // Validar duplicado de correo antes de intentar el insert
            const string emailExistsQuery = @"SELECT 1 FROM users.user WHERE Email = @Email LIMIT 1;";
            var emailExists = await _conn.ExecuteScalarAsync<int?>(emailExistsQuery, new { entity.Email });
            if (emailExists.HasValue)
            {
                return Result.Failure("El correo ya está registrado.");
            }

            using var transaction = _conn.BeginTransaction();

            const string query = @"
                INSERT INTO users.user
                    (Name, FirstLastname, SecondLastname, DateOfBirth, Ci, UserRole, HireDate, MonthlySalary, Specialization, Email, Password, CreatedAt)
                VALUES 
                    (@Name, @FirstLastname, @SecondLastname, @DateOfBirth, @Ci, @UserRole, @HireDate, @MonthlySalary, @Specialization, @Email, crypt(@Password, gen_salt('bf')), @CreatedAt)
                RETURNING Id;";

            entity.Id = await _conn.ExecuteScalarAsync<int>(query, entity, transaction);

            if (entity.Id == 0)
            {
                transaction.Rollback();
                return Result.Failure("Error al crear usuario");
            }

            transaction.Commit();
            return Result.Success();
        }

        public async Task<Result> Update(User entity)
        {
            using var transaction = _conn.BeginTransaction();

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
                    LastModification = @LastModification
                WHERE Id = @Id;";

            var affected = await _conn.ExecuteAsync(updateQuery, entity, transaction);

            if (affected == 0)
            {
                transaction.Rollback();
                return Result.Failure("No se encontró el usuario para actualizar.");
            }

            transaction.Commit();
            return Result.Success();
        }

        public async Task<Result> DeleteById(int id)
        {
            const string query = @"UPDATE users.user SET IsActive = false, LastModification = @LastModification WHERE Id = @Id;";

            var affected = await _conn.ExecuteAsync(query, new { Id = id, LastModification = DateTime.UtcNow });

            return affected <= 0
                ? Result.Failure("No se encontró el usuario a eliminar.")
                : Result.Success();
        }

        public async Task<Result<User>> GetByEmail(string email)
        {
            const string query = @"SELECT * FROM users.user WHERE IsActive = true AND Email = @Email;";

            var user = await _conn.QuerySingleOrDefaultAsync<User>(query, new { Email = email });

            return user is null
                ? Result<User>.Failure("Usuario no encontrado.")
                : Result<User>.Success(user);
        }

        public async Task<Result> UpdatePassword(int id, string password)
        {
            const string query = @"UPDATE users.user SET Password = crypt(@Password, gen_salt('bf')), MustChangePassword = false WHERE Id = @Id;";

            var affected = await _conn.ExecuteAsync(query, new { Id = id, Password = password });

            return affected <= 0
                ? Result.Failure("Error al actualizar contraseña.")
                : Result.Success();
        }
    }
}
