using CadastroClientes.Domain.Entities;

namespace CadastroClientes.Domain.IRepositories;

public interface IClientRepository
{
    Task AddAsync(Client client, CancellationToken cancellationToken);

    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
