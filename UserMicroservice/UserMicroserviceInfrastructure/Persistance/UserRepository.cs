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

        public async Task<Result> Create(User entity, string? userEmail = null)
        {
            const string emailExistsQuery = @"SELECT 1 FROM users.user WHERE Email = @Email LIMIT 1;";
            var emailExists = await _conn.ExecuteScalarAsync<int?>(emailExistsQuery, new { entity.Email });
            if (emailExists.HasValue)
            {
                return Result.Failure("El correo ya está registrado.");
            }

            using var transaction = _conn.BeginTransaction();

            const string query = @"
                INSERT INTO users.user
                    (Name, FirstLastname, SecondLastname, DateOfBirth, Ci, UserRole, HireDate, MonthlySalary, Specialization, Email, Password, CreatedAt, created_by)
                VALUES 
                    (@Name, @FirstLastname, @SecondLastname, @DateOfBirth, @Ci, @UserRole, @HireDate, @MonthlySalary, @Specialization, @Email, crypt(@Password, gen_salt('bf')), @CreatedAt, @created_by)
                RETURNING Id;";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", entity.Name);
            parameters.Add("@FirstLastname", entity.FirstLastname);
            parameters.Add("@SecondLastname", entity.SecondLastname);
            parameters.Add("@DateOfBirth", entity.DateOfBirth);
            parameters.Add("@Ci", entity.Ci);
            parameters.Add("@UserRole", entity.UserRole);
            parameters.Add("@HireDate", entity.HireDate);
            parameters.Add("@MonthlySalary", entity.MonthlySalary);
            parameters.Add("@Specialization", entity.Specialization);
            parameters.Add("@Email", entity.Email);
            parameters.Add("@Password", entity.Password);
            parameters.Add("@CreatedAt", entity.CreatedAt);
            parameters.Add("@created_by", userEmail);

            entity.Id = await _conn.ExecuteScalarAsync<int>(query, parameters, transaction);

            if (entity.Id == 0)
            {
                transaction.Rollback();
                return Result.Failure("Error al crear usuario");
            }

            transaction.Commit();
            return Result.Success();
        }

        public async Task<Result> Update(User entity, string? userEmail = null)
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
                    LastModification = @LastModification,
                    modified_by = @modified_by
                WHERE Id = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", entity.Id);
            parameters.Add("@Name", entity.Name);
            parameters.Add("@FirstLastname", entity.FirstLastname);
            parameters.Add("@SecondLastname", entity.SecondLastname);
            parameters.Add("@DateOfBirth", entity.DateOfBirth);
            parameters.Add("@Ci", entity.Ci);
            parameters.Add("@UserRole", entity.UserRole);
            parameters.Add("@HireDate", entity.HireDate);
            parameters.Add("@MonthlySalary", entity.MonthlySalary);
            parameters.Add("@Specialization", entity.Specialization);
            parameters.Add("@LastModification", entity.LastModification);
            parameters.Add("@modified_by", userEmail);

            var affected = await _conn.ExecuteAsync(updateQuery, parameters, transaction);

            if (affected == 0)
            {
                transaction.Rollback();
                return Result.Failure("No se encontró el usuario para actualizar.");
            }

            transaction.Commit();
            return Result.Success();
        }

        public async Task<Result> DeleteById(int id, string? userEmail = null)
        {
            const string query = @"UPDATE users.user 
                                   SET IsActive = false, LastModification = @LastModification, modified_by = @modified_by 
                                   WHERE Id = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@LastModification", DateTime.UtcNow);
            parameters.Add("@modified_by", userEmail);

            var affected = await _conn.ExecuteAsync(query, parameters);

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

        public async Task<Result> UpdatePassword(int id, string password, string? userEmail = null)
        {
            const string query = @"
                UPDATE users.user
                SET Password = crypt(@Password, gen_salt('bf')),
                    MustChangePassword = false,
                    LastModification = @LastModification,
                    modified_by = @modified_by
                WHERE Id = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@Password", password);
            parameters.Add("@LastModification", DateTime.UtcNow);
            parameters.Add("@modified_by", userEmail);

            var affected = await _conn.ExecuteAsync(query, parameters);

            return affected <= 0
                ? Result.Failure("Error al actualizar contraseña.")
                : Result.Success();
        }
    }
}
