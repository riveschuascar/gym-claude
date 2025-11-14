using UserMicroservice.Domain.Entities;
using UserMicroservice.Domain.Shared;

namespace UserMicroservice.Domain.Ports
{
    public interface IUserRepository : IRepository
    {
        Task<Result<User>> GetByEmail(string email);
        Task<Result> UpdatePassword(string email, string password);
    }
}