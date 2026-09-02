namespace CadastroClientes.Application.Messaging.Events;

public sealed record ClienteCadastradoEvent
{
    public Guid EventId { get; init; }

    public DateTimeOffset OccurredAt { get; init; }

    public Guid ClientId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Cpf { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public int Score { get; init; }
}
