using System.ComponentModel.DataAnnotations;
using CadastroClientes.Application.DTOs;
using MediatR;

namespace CadastroClientes.Application.Commands;

public sealed record CreateClientCommand : IRequest<ClientDto>
{
    [property: Required(ErrorMessage = "Nome é obrigatório.")]
    public string? Name { get; init; }

    [property: Required(ErrorMessage = "CPF é obrigatório.")]
    [property: RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$|^\d{11}$", ErrorMessage = "CPF inválido.")]
    public string? Cpf { get; init; }

    [property: Required(ErrorMessage = "E-mail é obrigatório.")]
    [property: EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; init; }
}
