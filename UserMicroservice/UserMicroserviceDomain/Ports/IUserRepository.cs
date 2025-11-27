using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Domain.Ports
{
    public interface IUserRepository
    {
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result<User>> GetById(int id);
        Task<Result> Create(User entity, string? userEmail = null);
        Task<Result> Update(User entity, string? userEmail = null);
        Task<Result> DeleteById(int id, string? userEmail = null);
        Task<Result<User>> GetByEmail(string email);
        Task<Result> UpdatePassword(int id, string password, string? userEmail = null);
    }
}
