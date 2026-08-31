using System.ComponentModel.DataAnnotations;
using CadastroClientes.Application.Commands;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Domain.Exceptions;
using CadastroClientes.UnitTests.Support;

namespace CadastroClientes.UnitTests.Application;

public class CreateClientCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistAndReturnDto()
    {
        // Arrange
        var repository = new FakeClientRepository();
        var publisher = new FakeMessagePublisher();
        var handler = new CreateClientCommandHandler(repository, publisher);
        var command = new CreateClientCommand
        {
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            Email = "maria.silva@email.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<ClientDto>(result);
        Assert.Equal("Maria Silva", result.Name);
        Assert.Equal("52998224725", result.Cpf);
        Assert.Equal("maria.silva@email.com", result.Email);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(1, repository.AddCalls);
        Assert.Equal(1, publisher.PublishCalls);
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
        var repository = new FakeClientRepository();
        var publisher = new FakeMessagePublisher();
        var handler = new CreateClientCommandHandler(repository, publisher);
        var command = new CreateClientCommand
        {
            Name = name,
            Cpf = cpf,
            Email = email
        };

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<DomainValidationException>(act);
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(0, repository.AddCalls);
        Assert.Equal(0, publisher.PublishCalls);
    }

    [Fact]
    public void CreateClientCommand_WithInvalidAnnotations_ShouldFailValidation()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = null,
            Cpf = "123",
            Email = "invalid-email"
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
