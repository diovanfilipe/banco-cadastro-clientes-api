namespace CadastroClientes.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(
        TMessage message,
        string routingKey,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}
