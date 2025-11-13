using Users.Domain.Ports;
using Users.Domain.Shared;
using Users.Domain.Entities;
using Users.Domain.Rules;
using Users.Domain.Validators;
using Users.Application.Interfaces;

namespace Users.Application.Services
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
            var res = await repo.GetById(id);
            if (!res.IsSuccess)
            {
                return Result<User>.Failure($"No se encontró el usuario con ID {id}.");
            }
            return Result<User>.Success(res.Value!);
        }

        public async Task<Result<User>> Create(User newUser)
        {
            var res = UserValidator.Validate(newUser);
            if (!res.IsSuccess)
            {
                return res;
            }

            res = await repo.Create(newUser);
            return Result<User>.Success(res.Value!);
        }

        public async Task<Result<User>> Update(User userToUpdate)
        {
            var res = UserValidator.Validate(userToUpdate);
            if (!res.IsSuccess)
            {
                return res;
            }

            var updatedUser = await repo.Update(userToUpdate);
            if (updatedUser == null)
            {
                return Result<User>.Failure($"No se encontró el usuario con ID {userToUpdate.Id} para actualizar.");
            }

            return Result<User>.Success(updatedUser.Value!);
        }

        public async Task<Result<bool>> Delete(int userId)
        {
            var res = await repo.DeleteById(userId);
            if (!res.IsSuccess)
            {
                return Result<bool>.Failure($"No se pudo eliminar el usuario con ID {userId} (probablemente no se encontró).");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdatePassword(int userId, string currentPassword, string newPassword)
        {
            var passwordres = UserRules.PasswordRules(newPassword);
            if (!passwordres.IsSuccess)
            {
                return Result<bool>.Failure(passwordres.Error!);
            }

            var res = await repo.UpdatePassword(userId, newPassword);
            if (!res.IsSuccess)
            {
                return Result<bool>.Failure($"No se pudo actualizar la contraseña para el usuario con ID {userId}.");
            }
            return Result<bool>.Success(true);
        }
    }
}