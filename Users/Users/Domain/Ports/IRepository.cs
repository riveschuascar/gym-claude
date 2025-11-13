using Users.Domain.Shared;
using Users.Domain.Entities;

namespace Users.Domain.Ports
{
    public interface IRepository
    {
        Task<Result<User>> GetById(int id);
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result<User>> Create(User entity);
        Task<Result<User>> Update(User entity);
        Task<Result> DeleteById(int id);
    }
}