using UserMicroservice.Domain.Ports;
using UserMicroservice.Domain.Shared;
using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Rules;
using UserMicroservice.Domain.Validators;
using UserMicroservice.Application.Interfaces;

namespace UserMicroservice.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repo;

        public UserService(IUserRepository userRepository)
        {
            repo = userRepository;
        }

        public async Task<Result<IEnumerable<User>>> GetAll()
        {
            return await repo.GetAll();
        }

        public async Task<Result<User>> GetById(int id)
        {
            var validation = await repo.GetById(id);

            if (!validation.IsSuccess)
                return Result<User>.Failure($"No se encontró el usuario con ID {id}.");

            return Result<User>.Success(validation.Value!);
        }

        public async Task<Result> Create(User newUser, string? userEmail = null)
        {
            newUser.CreatedAt = DateTime.Now;
            newUser.CreatePassword();

            var validation = UserSpecification.Create(newUser);

            if (!validation.IsSuccess)
            {
                return Result.Failure(validation.Error!);
            }

            return await repo.Create(newUser, userEmail);
        }

        public async Task<Result> Update(User userToUpdate, string? userEmail = null)
        {
            userToUpdate.LastModification = DateTime.Now;
            var validation = UserSpecification.Update(userToUpdate);

            if (!validation.IsSuccess)
            {
                return Result.Failure(validation.Error!);
            }

            return await repo.Update(userToUpdate, userEmail);
        }

        public async Task<Result> Delete(int userId, string? userEmail = null)
        {
            return await repo.DeleteById(userId, userEmail);
        }

        public async Task<Result<User>> GetByEmail(string email)
        {
            var validation = UserRules.EmailRules(email);
            
            if (!validation.IsSuccess)
                return Result<User>.Failure(validation.Error!);
            
            return await repo.GetByEmail(email);
        }

        public async Task<Result> UpdatePassword(int userId, string newPassword, string? userEmail = null)
        {
            var validation = UserRules.PasswordRules(newPassword);

            if (!validation.IsSuccess)
            {
                return Result.Failure(validation.Error!);
            }

            return await repo.UpdatePassword(userId, newPassword, userEmail);
        }
    }
}
