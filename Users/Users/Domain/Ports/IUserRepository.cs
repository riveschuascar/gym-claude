using Domain.Entities;

namespace Domain.Ports
{
    public interface IUserRepository : IRepository
    {
        Task<Result<User>> GetByEmail(string email);
        Task<Result> UpdatePassword(int id, string password);
    }
}