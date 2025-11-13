using Domain.Shared;
using Domain.Entities;

namespace Domain.Ports
{
    public interface IRepository
    {
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result<User>> GetById(int id);
        Task<Result<User>> Create(User entity);
        Task<Result<User>> Update(User entity);
        Task<Result> DeleteById(int id);
    }
}