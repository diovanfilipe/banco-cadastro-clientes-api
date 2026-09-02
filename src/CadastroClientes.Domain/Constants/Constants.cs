namespace CadastroClientes.Domain.Constants;

public static class Constants
{
    public static class RabbitMqConstantes
    {
        public const string ExchangeType = "direct";

        public const string ClienteCadastradoExchangeName = "clientes.events";

        public const string ClienteCadastradoRoutingKey = "ClienteCadastradoEvent";
    }
}
