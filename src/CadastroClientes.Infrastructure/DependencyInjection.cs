using CadastroClientes.Application.Abstractions;
using CadastroClientes.Infrastructure.Persistence;
using CadastroClientes.Infrastructure.Repositories;
using CadastroClientes.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroClientes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCadastroClientesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CadastroClientes")
            ?? "Data Source=file:CadastroClientesDb?mode=memory&cache=shared";

        services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddSingleton<SqliteSchemaInitializer>();
        services.AddScoped<IClientRepository, ClientRepository>();

        var rabbitSection = configuration.GetSection("RabbitMq");
        services.AddSingleton(new RabbitMqOptions
        {
            HostName = rabbitSection["HostName"] ?? "localhost",
            Port = int.TryParse(rabbitSection["Port"], out var port) ? port : 5672,
            VirtualHost = rabbitSection["VirtualHost"] ?? "/",
            UserName = rabbitSection["UserName"] ?? "guest",
            Password = rabbitSection["Password"] ?? "guest",
            ExchangeName = rabbitSection["ExchangeName"] ?? "clientes.events",
            ExchangeType = rabbitSection["ExchangeType"] ?? "direct"
        });
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();

        return services;
    }
}
