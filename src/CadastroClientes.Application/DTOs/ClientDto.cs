namespace CadastroClientes.Application.DTOs;

public sealed record ClientDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Cpf { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}
