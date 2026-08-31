using CadastroClientes.Domain.Entities;
using CadastroClientes.Infrastructure.Persistence;
using CadastroClientes.Infrastructure.Repositories;

namespace CadastroClientes.UnitTests.Infrastructure;

public class ClientRepositoryTests
{
    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ShouldPersistAndReturnClient()
    {
        // Arrange
        using var factory = CreateFactory();
        var initializer = new SqliteSchemaInitializer(factory);
        initializer.Initialize();
        var repository = new ClientRepository(factory);
        var client = Client.Create(
            "Maria Silva",
            "529.982.247-25",
            "maria.silva@email.com",
            new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero));

        // Act
        await repository.AddAsync(client, CancellationToken.None);
        var result = await repository.GetByIdAsync(client.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(client.Id, result!.Id);
        Assert.Equal(client.Name, result.Name);
        Assert.Equal(client.Cpf.Value, result.Cpf.Value);
        Assert.Equal(client.Email.Value, result.Email.Value);
        Assert.Equal(client.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenClientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var factory = CreateFactory();
        var initializer = new SqliteSchemaInitializer(factory);
        initializer.Initialize();
        var repository = new ClientRepository(factory);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    private static SqliteConnectionFactory CreateFactory()
    {
        var connectionString = $"Data Source=file:cadastro-clientes-{Guid.NewGuid():N}?mode=memory&cache=shared";
        return new SqliteConnectionFactory(connectionString);
    }
}
