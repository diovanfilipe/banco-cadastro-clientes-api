using CadastroClientes.Application.Abstractions;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Domain.Entities;
using MediatR;

namespace CadastroClientes.Application.Commands;

public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _clientRepository;

    public CreateClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = Client.Create(request.Name, request.Cpf, request.Email);

        await _clientRepository.AddAsync(client, cancellationToken);

        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Cpf = client.Cpf.Value,
            Email = client.Email.Value,
            CreatedAt = client.CreatedAt
        };
    }
}
