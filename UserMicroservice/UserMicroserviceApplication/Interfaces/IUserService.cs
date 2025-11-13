using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetById(int id);
        Task<Result<IEnumerable<User>>> GetAll(); 
        Task<Result<User>> Create(User newUser);
        Task<Result<User>> Update(User userToUpdate);
        Task<Result<bool>> Delete(int userId);
        Task<Result<bool>> UpdatePassword(int userId, string currentPassword, string newPassword);
    }
}