using ClientMicroservice.Domain.Entities;

namespace ClientMicroservice.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client> CreateAsync(Client client, string? userEmail = null);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task<Client?> UpdateAsync(Client client, string? userEmail = null);
    Task<bool> DeleteByIdAsync(int id, string? userEmail = null);
}
