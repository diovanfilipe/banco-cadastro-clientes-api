using System.ComponentModel.DataAnnotations;
using CadastroClientes.Domain.Exceptions;

namespace CadastroClientes.Domain.ValueObjects;

public sealed record Email
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? value)
    {
        var candidate = RequireValue(value, "E-mail");

        if (!EmailAddressAttribute.IsValid(candidate))
        {
            throw new DomainValidationException("E-mail inválido.");
        }

        return new Email(candidate);
    }

    private static string RequireValue(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} é obrigatório.");
        }

        return value.Trim();
    }

    public override string ToString() => Value;
}
