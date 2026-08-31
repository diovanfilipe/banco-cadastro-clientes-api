using CadastroClientes.Application.Abstractions;

namespace CadastroClientes.UnitTests.Support;

internal sealed class FakeMessagePublisher : IMessagePublisher
{
    public object? LastMessage { get; private set; }

    public int PublishCalls { get; private set; }

    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        PublishCalls++;
        LastMessage = message;
        return Task.CompletedTask;
    }
}
