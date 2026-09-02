using System.ComponentModel.DataAnnotations;
using CadastroClientes.Application.DTOs;
using MediatR;

namespace CadastroClientes.Application.Commands;

public sealed record CreateClientCommand : IRequest<ClientDto>
{
    [property: Required(ErrorMessage = "Nome é obrigatório.")]
    public required string Name { get; init; }

    [property: Required(ErrorMessage = "CPF é obrigatório.")]
    [property: RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$|^\d{11}$", ErrorMessage = "CPF inválido.")]
    public required string Cpf { get; init; }

    [property: Required(ErrorMessage = "E-mail é obrigatório.")]
    [property: EmailAddress(ErrorMessage = "E-mail inválido.")]
    public required string Email { get; init; }

    [property: Required(ErrorMessage = "Score é obrigatório.")]
    [property: Range(0, 1000, ErrorMessage = "Score deve estar entre 0 e 1000.")]
    public required int? Score { get; init; }
}
