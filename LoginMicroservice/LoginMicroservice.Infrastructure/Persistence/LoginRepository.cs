using System.Data;
using Dapper;
using LoginMicroservice.Domain.Entities;
using LoginMicroservice.Domain.Ports;
using LoginMicroservice.Domain.Shared;

namespace LoginMicroservice.Infrastructure.Persistence
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IDbConnection _connection;

        public LoginRepository(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public async Task<Result<AuthenticatedUser>> AuthenticateAsync(string email, string password)
        {
            const string query = @"
                SELECT 
                    Id,
                    Name,
                    FirstLastname,
                    SecondLastname,
                    Email,
                    UserRole,
                    MustChangePassword,
                    Password
                FROM users.user
                WHERE IsActive = true 
                  AND lower(Email) = lower(@Email);";

            try
            {
                var dbUser = await _connection.QuerySingleOrDefaultAsync<DbUser>(query, new { Email = email });

                if (dbUser == null)
                    return Result<AuthenticatedUser>.Failure("Invalid email or password.");

                // Validate bcrypt (crypt) or legacy SHA256 hex hashes.
                const string passwordCheck = @"
                    SELECT
                        (crypt(@Password, @Stored) = @Stored)
                        OR (@Stored = encode(digest(@Password, 'sha256'), 'hex'));";

                var isValid = await _connection.ExecuteScalarAsync<bool>(passwordCheck, new { Password = password, Stored = dbUser.Password });

                if (!isValid)
                    return Result<AuthenticatedUser>.Failure("Invalid email or password.");

                var fullName = $"{dbUser.Name} {dbUser.FirstLastname} {dbUser.SecondLastname}".Trim();

                var user = new AuthenticatedUser
                {
                    Id = dbUser.Id,
                    FullName = fullName,
                    Email = dbUser.Email,
                    Role = string.IsNullOrWhiteSpace(dbUser.UserRole) ? "User" : dbUser.UserRole,
                    MustChangePassword = dbUser.MustChangePassword
                };

                return Result<AuthenticatedUser>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<AuthenticatedUser>.Failure($"Error while authenticating: {ex.Message}");
            }
        }

        private class DbUser
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string FirstLastname { get; set; } = string.Empty;
            public string? SecondLastname { get; set; }
            public string Email { get; set; } = string.Empty;
            public string UserRole { get; set; } = string.Empty;
            public bool MustChangePassword { get; set; }
            public string Password { get; set; } = string.Empty;
        }
    }
}
