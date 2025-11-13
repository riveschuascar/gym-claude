using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetById(int id);
        Task<IEnumerable<User>> GetAll(); 
        Task<Result<User>> Create(User newUser);
        Task<Result<User>> Update(User userToUpdate);
        Task<Result<bool>> Delete(int userId);
        Task<Result<bool>> UpdatePassword(int userId, string currentPassword, string newPassword);
    }
}