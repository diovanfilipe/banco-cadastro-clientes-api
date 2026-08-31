using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CadastroClientes.UnitTests.Support;

internal sealed class FakeSender
{
    public FakeSender()
    {
        Sender = DispatchProxy.Create<ISender, SenderProxy>();
        SenderProxy.Register(Sender, this);
    }

    public ISender Sender { get; }

    public object? LastRequest { get; internal set; }

    public CancellationToken LastCancellationToken { get; internal set; }

    public object? Response { get; set; }

    private class SenderProxy : DispatchProxy
    {
        private static readonly ConditionalWeakTable<object, FakeSender> States = new();

        public static void Register(object proxy, FakeSender state)
        {
            States.Add(proxy, state);
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null || !States.TryGetValue(this, out var state))
            {
                throw new InvalidOperationException("Fake sender was not initialized correctly.");
            }

            state.LastRequest = args is { Length: > 0 } ? args[0] : null;
            state.LastCancellationToken = args is { Length: > 1 } && args[1] is CancellationToken token
                ? token
                : default;

            if (targetMethod.ReturnType == typeof(Task))
            {
                return Task.CompletedTask;
            }

            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var responseType = targetMethod.ReturnType.GenericTypeArguments[0];

                if (responseType == typeof(object))
                {
                    return Task.FromResult(state.Response);
                }

                var fromResult = typeof(Task)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(method => method.Name == nameof(Task.FromResult) && method.IsGenericMethodDefinition)
                    .MakeGenericMethod(responseType);

                return fromResult.Invoke(null, [state.Response])!;
            }

            if (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
            {
                return CreateEmptyAsyncEnumerable(targetMethod.ReturnType.GenericTypeArguments[0]);
            }

            return state.Response;
        }

        private static object CreateEmptyAsyncEnumerable(Type itemType)
        {
            var method = typeof(SenderProxy)
                .GetMethod(nameof(CreateEmptyAsyncEnumerableCore), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(itemType);

            return method.Invoke(null, null)!;
        }

        private static async IAsyncEnumerable<T> CreateEmptyAsyncEnumerableCore<T>()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
