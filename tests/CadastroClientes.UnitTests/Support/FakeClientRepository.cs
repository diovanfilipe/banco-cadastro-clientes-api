using CadastroClientes.Application.Abstractions;
using CadastroClientes.Domain.Entities;

namespace CadastroClientes.UnitTests.Support;

internal sealed class FakeClientRepository : IClientRepository
{
    private readonly Dictionary<Guid, Client> _clients = new();

    public int AddCalls { get; private set; }

    public Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        AddCalls++;
        _clients[client.Id] = client;
        return Task.CompletedTask;
    }

    public Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _clients.TryGetValue(id, out var client);
        return Task.FromResult(client);
    }

    public void Seed(Client client)
    {
        _clients[client.Id] = client;
    }
}
