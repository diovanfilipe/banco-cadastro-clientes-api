using CadastroClientes.Application.Commands;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Application.Queries;
using MediatR;
using System.Runtime.CompilerServices;

namespace CadastroClientes.UnitTests.Support;

internal sealed class FakeSender : ISender
{
    public object? LastRequest { get; private set; }

    public CancellationToken LastCancellationToken { get; private set; }

    public object? Response { get; set; }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        LastCancellationToken = cancellationToken;
        return Task.FromResult((TResponse)Response!);
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        LastCancellationToken = cancellationToken;
        return Task.FromResult(Response);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        LastRequest = request;
        LastCancellationToken = cancellationToken;
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }

    public async IAsyncEnumerable<object?> CreateStream(
        object request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}
