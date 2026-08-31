using System.Text.RegularExpressions;
using CadastroClientes.Domain.Exceptions;

namespace CadastroClientes.Domain.ValueObjects;

public sealed record Cpf
{
    private static readonly Regex NonDigitRegex = new(@"[^\d]", RegexOptions.Compiled);

    public string Value { get; }

    private Cpf(string value)
    {
        Value = value;
    }

    public static Cpf Create(string? value)
    {
        var candidate = RequireValue(value, "CPF");
        var digits = NonDigitRegex.Replace(candidate, string.Empty);

        if (digits.Length != 11)
        {
            throw new DomainValidationException("CPF inválido.");
        }

        if (digits.Distinct().Count() == 1)
        {
            throw new DomainValidationException("CPF inválido.");
        }

        if (!IsValidDigits(digits))
        {
            throw new DomainValidationException("CPF inválido.");
        }

        return new Cpf(digits);
    }

    private static string RequireValue(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} é obrigatório.");
        }

        return value.Trim();
    }

    private static bool IsValidDigits(string digits)
    {
        static int CalculateDigit(ReadOnlySpan<char> input, int multiplierStart)
        {
            var sum = 0;
            var multiplier = multiplierStart;

            for (var index = 0; index < input.Length; index++)
            {
                sum += (input[index] - '0') * multiplier;
                multiplier--;
            }

            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        var firstDigit = CalculateDigit(digits.AsSpan(0, 9), 10);
        if (firstDigit != digits[9] - '0')
        {
            return false;
        }

        var secondDigit = CalculateDigit(digits.AsSpan(0, 10), 11);
        return secondDigit == digits[10] - '0';
    }

    public override string ToString() => Value;
}
