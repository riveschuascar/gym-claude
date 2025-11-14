using ClientMicroservice.Domain.Entities;
using ClientMicroservice.Domain.Interfaces;

namespace ClientMicroservice.Application.Services;

public class ClientService
{
    private readonly IClientRepository _repo;

    public ClientService(IClientRepository repo)
    {
        _repo = repo;
    }

    public async Task<Client> CreateAsync(Client client)
    {
        client.CreatedAt = DateTime.UtcNow;
        client.IsActive = true;
        return await _repo.CreateAsync(client);
    }

    public Task<IEnumerable<Client>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Client?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<Client?> UpdateAsync(Client client)
    {
        return _repo.UpdateAsync(client);
    }

    public Task<bool> DeleteByIdAsync(int id) => _repo.DeleteByIdAsync(id);
}
