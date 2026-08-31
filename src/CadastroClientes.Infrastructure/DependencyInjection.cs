using CadastroClientes.Application.Abstractions;
using CadastroClientes.Infrastructure.Persistence;
using CadastroClientes.Infrastructure.Repositories;
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

        return services;
    }
}
