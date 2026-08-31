using CadastroClientes.Application.Queries;
using CadastroClientes.Domain.Entities;
using CadastroClientes.UnitTests.Support;

namespace CadastroClientes.UnitTests.Application;

public class GetClientByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenClientExists_ShouldReturnDto()
    {
        // Arrange
        var repository = new FakeClientRepository();
        var handler = new GetClientByIdQueryHandler(repository);
        var client = Client.Create("Maria Silva", "529.982.247-25", "maria.silva@email.com", new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero));
        repository.Seed(client);
        var query = new GetClientByIdQuery(client.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(client.Id, result!.Id);
        Assert.Equal(client.Name, result.Name);
        Assert.Equal(client.Cpf.Value, result.Cpf);
        Assert.Equal(client.Email.Value, result.Email);
        Assert.Equal(client.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task Handle_WhenClientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var repository = new FakeClientRepository();
        var handler = new GetClientByIdQueryHandler(repository);
        var query = new GetClientByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
