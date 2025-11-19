using UserMicroservice.Domain.Shared;
using UserMicroservice.Domain.Entities;

namespace UserMicroservice.Domain.Ports
{
    public interface IRepository
    {
        Task<Result<User>> GetById(int id);
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result> Create(User entity);
        Task<Result> Update(User entity);
        Task<Result> DeleteById(int id);
    }
}