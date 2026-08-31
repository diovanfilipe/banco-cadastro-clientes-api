using CadastroClientes.Domain.Exceptions;
using CadastroClientes.Domain.ValueObjects;

namespace CadastroClientes.UnitTests.Domain;

public class CpfTests
{
    [Fact]
    public void Create_WithValidCpf_ShouldNormalizeValue()
    {
        // Arrange
        const string cpf = "529.982.247-25";

        // Act
        var valueObject = Cpf.Create(cpf);

        // Assert
        Assert.Equal("52998224725", valueObject.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingCpf_ShouldThrow(string? cpf)
    {
        // Arrange
        var act = () => Cpf.Create(cpf);

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
        var act = () => Cpf.Create(cpf);

        // Assert
        var exception = Assert.Throws<DomainValidationException>(act);
        Assert.Equal("CPF inválido.", exception.Message);
    }
}
