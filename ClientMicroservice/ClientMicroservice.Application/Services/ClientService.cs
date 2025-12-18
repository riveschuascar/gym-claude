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

    public async Task<Client> CreateAsync(Client client, string? userEmail)
    {
        var validation = Domain.Validators.ClientValidator.Validate(client);
        if (!validation.IsValid)
            throw new ArgumentException(validation.Error);

        client.CreatedAt = DateTime.UtcNow;
        client.IsActive = true;
        return await _repo.CreateAsync(client, userEmail);
    }

    public Task<IEnumerable<Client>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Client?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<Client?> UpdateAsync(Client client, string? userEmail)
    {
        var validation = Domain.Validators.ClientValidator.Validate(client);
        if (!validation.IsValid)
            throw new ArgumentException(validation.Error);

        return await _repo.UpdateAsync(client, userEmail);
    }

    public Task<bool> DeleteByIdAsync(int id, string? userEmail) => _repo.DeleteByIdAsync(id, userEmail);
}
