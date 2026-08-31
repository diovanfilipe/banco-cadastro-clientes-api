using CadastroClientes.Application.Commands;
using CadastroClientes.Infrastructure;
using CadastroClientes.Infrastructure.Persistence;
using CadastroClientes.Api.Middleware;
using MediatR;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CadastroClientes API",
        Version = "v1",
        Description = "Microsserviço de cadastro de clientes."
    });
});
builder.Services.AddHealthChecks();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateClientCommand>());
builder.Services.AddCadastroClientesInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var schemaInitializer = scope.ServiceProvider.GetRequiredService<SqliteSchemaInitializer>();
    schemaInitializer.Initialize();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CadastroClientes API v1");
});

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
