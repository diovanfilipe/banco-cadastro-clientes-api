using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.Exceptions;

namespace CadastroClientes.UnitTests.Domain;

public class ClientTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateClient()
    {
        // Arrange
        var name = "Maria Silva";
        var cpf = "529.982.247-25";
        var email = "maria.silva@email.com";
        var createdAt = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);

        // Act
        var client = Client.Create(name, cpf, email, createdAt);

        // Assert
        Assert.NotEqual(Guid.Empty, client.Id);
        Assert.Equal(name, client.Name);
        Assert.Equal("52998224725", client.Cpf.Value);
        Assert.Equal(email, client.Email.Value);
        Assert.Equal(createdAt, client.CreatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingName_ShouldThrow(string? name)
    {
        // Arrange
        var cpf = "529.982.247-25";
        var email = "maria.silva@email.com";

        // Act
        var act = () => Client.Create(name, cpf, email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("Nome é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingCpf_ShouldThrow(string? cpf)
    {
        // Arrange
        var name = "Maria Silva";
        var email = "maria.silva@email.com";

        // Act
        var act = () => Client.Create(name, cpf, email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("CPF é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("111.111.111-11")]
    [InlineData("123.456.789-00")]
    public void Create_WithInvalidCpfFormat_ShouldThrow(string? cpf)
    {
        // Arrange
        var name = "Maria Silva";
        var email = "maria.silva@email.com";

        // Act
        var act = () => Client.Create(name, cpf, email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("CPF inválido.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingEmail_ShouldThrow(string? email)
    {
        // Arrange
        var name = "Maria Silva";
        var cpf = "529.982.247-25";

        // Act
        var act = () => Client.Create(name, cpf, email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("E-mail é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("maria.silva@")]
    public void Create_WithInvalidEmailFormat_ShouldThrow(string? email)
    {
        // Arrange
        var name = "Maria Silva";
        var cpf = "529.982.247-25";

        // Act
        var act = () => Client.Create(name, cpf, email);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("E-mail inválido.", exception.Message);
    }
}
