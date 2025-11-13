using Users.Domain.Entities;
using Users.Domain.Shared;

namespace Users.Domain.Ports
{
    public interface IUserRepository : IRepository
    {
        Task<Result<User>> GetByEmail(string email);
        Task<Result> UpdatePassword(int id, string password);
    }
}