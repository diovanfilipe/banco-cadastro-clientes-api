using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.IRepositories;
using Dapper;

namespace CadastroClientes.Infrastructure.Repositories;

public sealed class ClientRepository : IClientRepository
{
    private readonly Persistence.ISqliteConnectionFactory _connectionFactory;

    public ClientRepository(Persistence.ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO clients (id, name, cpf, email, created_at)
            VALUES (@Id, @Name, @Cpf, @Email, @CreatedAt);
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = client.Id.ToString(),
            client.Name,
            Cpf = client.Cpf.Value,
            Email = client.Email.Value,
            CreatedAt = client.CreatedAt.ToString("O")
        }, cancellationToken: cancellationToken));
    }

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id, name, cpf, email, created_at AS CreatedAt
            FROM clients
            WHERE id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<ClientRow>(
            new CommandDefinition(sql, new { Id = id.ToString() }, cancellationToken: cancellationToken));

        if (row is null)
        {
            return null;
        }

        return Client.Reconstitute(
            Guid.Parse(row.Id),
            row.Name,
            row.Cpf,
            row.Email,
            DateTimeOffset.Parse(row.CreatedAt));
    }

    private sealed record ClientRow(string Id, string Name, string Cpf, string Email, string CreatedAt);
}
