using CadastroClientes.Domain.Exceptions;
using CadastroClientes.Domain.ValueObjects;

namespace CadastroClientes.Domain.Entities;

public sealed class Client
{
    public Guid Id { get; }

    public string Name { get; }

    public Cpf Cpf { get; }

    public Email Email { get; }

    public DateTimeOffset CreatedAt { get; }

    private Client(Guid id, string name, Cpf cpf, Email email, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Cpf = cpf;
        Email = email;
        CreatedAt = createdAt;
    }

    public static Client Create(string? name, string? cpf, string? email, DateTimeOffset? createdAt = null)
    {
        var normalizedName = RequireValue(name, "Nome");
        var normalizedCpf = Cpf.Create(cpf);
        var normalizedEmail = Email.Create(email);

        return new Client(Guid.NewGuid(), normalizedName, normalizedCpf, normalizedEmail, createdAt ?? DateTimeOffset.UtcNow);
    }

    public static Client Reconstitute(Guid id, string? name, string? cpf, string? email, DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException("Id do cliente é obrigatório.");
        }

        var normalizedName = RequireValue(name, "Nome");
        var normalizedCpf = Cpf.Create(cpf);
        var normalizedEmail = Email.Create(email);

        return new Client(id, normalizedName, normalizedCpf, normalizedEmail, createdAt);
    }

    private static string RequireValue(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} é obrigatório.");
        }

        return value.Trim();
    }
}
