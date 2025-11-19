using ClientMicroservice.Domain.Entities;

namespace ClientMicroservice.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client> CreateAsync(Client client);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task<Client?> UpdateAsync(Client client);
    Task<bool> DeleteByIdAsync(int id);
}
