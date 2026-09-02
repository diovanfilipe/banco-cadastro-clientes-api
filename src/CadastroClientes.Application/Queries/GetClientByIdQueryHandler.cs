using CadastroClientes.Application.DTOs;
using CadastroClientes.Domain.IRepositories;
using MediatR;

namespace CadastroClientes.Application.Queries;

public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientRepository _clientRepository;

    public GetClientByIdQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (client is null)
        {
            return null;
        }

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
