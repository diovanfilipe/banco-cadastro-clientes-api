using CadastroClientes.Application.DTOs;
using MediatR;

namespace CadastroClientes.Application.Queries;

public sealed record GetClientByIdQuery(Guid Id) : IRequest<ClientDto?>;
