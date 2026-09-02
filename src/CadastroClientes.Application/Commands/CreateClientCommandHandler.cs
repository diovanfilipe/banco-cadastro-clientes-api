using CadastroClientes.Application.DTOs;
using CadastroClientes.Application.Messaging.Events;
using CadastroClientes.Domain.Entities;
using CadastroClientes.Domain.IRepositories;
using CadastroClientes.Application.Interfaces;
using CadastroClientes.Domain.Constants;
using MediatR;

namespace CadastroClientes.Application.Commands;

public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMessagePublisher _messagePublisher;

    public CreateClientCommandHandler(
        IClientRepository clientRepository,
        IMessagePublisher messagePublisher)
    {
        _clientRepository = clientRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = Client.Create(request.Name, request.Cpf, request.Email);

        await _clientRepository.AddAsync(client, cancellationToken);

        var integrationEvent = new ClienteCadastradoEvent
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            ClientId = client.Id,
            Name = client.Name,
            Cpf = client.Cpf.Value,
            Email = client.Email.Value,
            Score = request.Score!.Value
        };

        await _messagePublisher.PublishAsync(
            integrationEvent,
            Constants.RabbitMqConstantes.ClienteCadastradoRoutingKey,
            cancellationToken);

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
