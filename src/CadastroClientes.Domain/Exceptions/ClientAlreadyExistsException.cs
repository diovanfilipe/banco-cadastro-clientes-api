namespace CadastroClientes.Domain.Exceptions;

public sealed class ClientAlreadyExistsException : Exception
{
    public ClientAlreadyExistsException()
        : base("Cliente já cadastrado.")
    {
    }
}
