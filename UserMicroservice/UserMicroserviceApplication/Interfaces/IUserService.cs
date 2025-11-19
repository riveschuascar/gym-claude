using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetById(int id);
        Task<Result<IEnumerable<User>>> GetAll(); 
        Task<Result> Create(User newUser);
        Task<Result> Update(User userToUpdate);
        Task<Result> Delete(int userId);
        Task<Result<User>> GetByEmail(string email);
        Task<Result> UpdatePassword(int userId, string newPassword);
    }
}