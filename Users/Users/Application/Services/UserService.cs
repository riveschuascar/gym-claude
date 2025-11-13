using Domain.Ports;
using Domain.Rules;
using Application.Interfaces;

public class UserService : IUserService
{
    private readonly IUserRepository repo;

    public UserService(IUserRepository userRepository)
    {
        repo = userRepository;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await repo.GetAllAsync();
    }

    public async Task<Result<User>> GetById(int id)
    {
        var user = await repo.GetById(id);
        if (user == null)
        {
            return Result<User>.Failure($"No se encontró el usuario con ID {id}.");
        }
        return Result<User>.Success(user);
    }

    public async Task<Result<User>> Create(User newUser)
    {
        var validationResult = UserValidator.Validar(newUser);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        var createdUser = await repo.CreateAsync(newUser);
        return Result<User>.Success(createdUser);
    }

    public async Task<Result<User>> Update(User userToUpdate)
    {
        var validationResult = UserValidator.Validar(userToUpdate);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        var updatedUser = await repo.UpdateAsync(userToUpdate);
        if (updatedUser == null)
        {
            return Result<User>.Failure($"No se encontró el usuario con ID {userToUpdate.Id} para actualizar.");
        }

        return Result<User>.Success(updatedUser);
    }

    public async Task<Result<bool>> Delete(int userId)
    {
        var success = await repo.DeleteByIdAsync(userId);
        if (!success)
        {
            return Result<bool>.Failure($"No se pudo eliminar el usuario con ID {userId} (probablemente no se encontró).");
        }
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdatePassword(int userId, string currentPassword, string newPassword)
    {
        var passwordValidationResult = PasswordRules.Validar(newPassword);
        if (passwordValidationResult.IsFailure)
        {
            return Result<bool>.Failure(passwordValidationResult.Error);
        }

        var success = await repo.UpdatePasswordAsync(userId, newPassword);
        if (!success)
        {
            return Result<bool>.Failure($"No se pudo actualizar la contraseña para el usuario con ID {userId}.");
        }
        return Result<bool>.Success(true);
    }
}