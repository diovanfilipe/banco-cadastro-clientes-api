using CadastroClientes.Application.Queries;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.IRepositories;
using Moq;

namespace CadastroClientes.UnitTests.Application;

public class GetClientByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenClientExists_ShouldReturnDto()
    {
        // Arrange
        var client = Client.Create("Maria Silva", "529.982.247-25", "maria.silva@email.com", new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero));
        var repository = new Mock<IClientRepository>();
        repository
            .Setup(item => item.GetByIdAsync(client.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        var handler = new GetClientByIdQueryHandler(repository.Object);
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
        repository.Verify(
            item => item.GetByIdAsync(client.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenClientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var repository = new Mock<IClientRepository>();
        repository
            .Setup(item => item.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);
        var handler = new GetClientByIdQueryHandler(repository.Object);
        var query = new GetClientByIdQuery(clientId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        repository.Verify(
            item => item.GetByIdAsync(clientId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
