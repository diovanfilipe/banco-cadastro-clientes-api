using System.ComponentModel.DataAnnotations;
using CadastroClientes.Application.Commands;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Application.Messaging.Events;
using CadastroClientes.Domain.Exceptions;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.IRepositories;
using CadastroClientes.Domain.Constants;
using CadastroClientes.Application.Interfaces;
using Moq;

namespace CadastroClientes.UnitTests.Application;

public class CreateClientCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistAndReturnDto()
    {
        // Arrange
        var repository = new Mock<IClientRepository>();
        var publisher = new Mock<IMessagePublisher>();
        var handler = new CreateClientCommandHandler(repository.Object, publisher.Object);
        var command = new CreateClientCommand
        {
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            Email = "maria.silva@email.com",
            Score = 500
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<ClientDto>(result);
        Assert.Equal("Maria Silva", result.Name);
        Assert.Equal("52998224725", result.Cpf);
        Assert.Equal("maria.silva@email.com", result.Email);
        Assert.NotEqual(Guid.Empty, result.Id);
        repository.Verify(
            item => item.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Once);
        publisher.Verify(
            item => item.PublishAsync(
                It.Is<ClienteCadastradoEvent>(message =>
                    message.ClientId == result.Id &&
                    message.Name == result.Name &&
                    message.Cpf == result.Cpf &&
                    message.Email == result.Email &&
                    message.Score == 500),
                Constants.RabbitMqConstantes.ClienteCadastradoRoutingKey,
                It.IsAny<CancellationToken>()),
            Times.Once);

        var publishedEvent = Assert.IsType<ClienteCadastradoEvent>(
            publisher.Invocations.Single().Arguments[0]);
        Assert.NotEqual(Guid.Empty, publishedEvent.EventId);
        Assert.NotEqual(default, publishedEvent.OccurredAt);
        Assert.Equal(result.Id, publishedEvent.ClientId);
        Assert.Equal(result.Name, publishedEvent.Name);
        Assert.Equal(result.Cpf, publishedEvent.Cpf);
        Assert.Equal(result.Email, publishedEvent.Email);
    }

    [Theory]
    [InlineData(null, "529.982.247-25", "maria.silva@email.com", "Nome é obrigatório.")]
    [InlineData("Maria Silva", null, "maria.silva@email.com", "CPF é obrigatório.")]
    [InlineData("Maria Silva", "123", "maria.silva@email.com", "CPF inválido.")]
    [InlineData("Maria Silva", "529.982.247-25", null, "E-mail é obrigatório.")]
    [InlineData("Maria Silva", "529.982.247-25", "invalid-email", "E-mail inválido.")]
    public async Task Handle_WithInvalidData_ShouldThrow(string? name, string? cpf, string? email, string expectedMessage)
    {
        // Arrange
        var repository = new Mock<IClientRepository>();
        var publisher = new Mock<IMessagePublisher>();
        var handler = new CreateClientCommandHandler(repository.Object, publisher.Object);
        var command = new CreateClientCommand
        {
            Name = name!,
            Cpf = cpf!,
            Email = email!,
            Score = 500
        };

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<DomainValidationException>(act);
        Assert.Equal(expectedMessage, exception.Message);
        repository.Verify(
            item => item.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()),
            Times.Never);
        publisher.Verify(
            item => item.PublishAsync(
                It.IsAny<ClienteCadastradoEvent>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void CreateClientCommand_WithInvalidAnnotations_ShouldFailValidation()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = null!,
            Cpf = "123",
            Email = "invalid-email",
            Score = 500
        };
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(command, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, result => result.ErrorMessage == "Nome é obrigatório.");
        Assert.Contains(results, result => result.ErrorMessage == "CPF inválido.");
        Assert.Contains(results, result => result.ErrorMessage == "E-mail inválido.");
    }
}
