using CadastroClientes.Domain.Exceptions;
using CadastroClientes.Domain.ValueObjects;

namespace CadastroClientes.UnitTests.Domain;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldCreateValueObject()
    {
        // Arrange
        const string email = "cliente@email.com";

        // Act
        var valueObject = Email.Create(email);

        // Assert
        Assert.Equal(email, valueObject.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingEmail_ShouldThrow(string? email)
    {
        // Arrange
        var act = () => Email.Create(email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("E-mail é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("cliente@")]
    [InlineData("cliente.email.com")]
    public void Create_WithInvalidEmailFormat_ShouldThrow(string? email)
    {
        // Arrange
        var act = () => Email.Create(email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("E-mail inválido.", exception.Message);
    }
}
