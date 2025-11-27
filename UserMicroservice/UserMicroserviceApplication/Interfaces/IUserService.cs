using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result<User>> GetById(int id);
        Task<Result> Create(User newUser, string? userEmail = null);
        Task<Result> Update(User userToUpdate, string? userEmail = null);
        Task<Result> Delete(int userId, string? userEmail = null);
        Task<Result<User>> GetByEmail(string email);
        Task<Result> UpdatePassword(int userId, string newPassword, string? userEmail = null);
    }
}