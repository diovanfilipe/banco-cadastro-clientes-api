using CadastroClientes.Api.Controllers;
using CadastroClientes.Application.Commands;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Application.Queries;
using CadastroClientes.UnitTests.Support;
using Microsoft.AspNetCore.Mvc;

namespace CadastroClientes.UnitTests.Api;

public class ClientesControllerTests
{
    [Fact]
    public async Task Create_WithValidCommand_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var sender = new FakeSender
        {
            Response = new ClientDto
            {
                Id = Guid.NewGuid(),
                Name = "Maria Silva",
                Cpf = "52998224725",
                Email = "maria.silva@email.com",
                CreatedAt = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)
            }
        };
        var controller = new ClientesController(sender.Sender);
        var command = new CreateClientCommand
        {
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            Email = "maria.silva@email.com"
        };

        // Act
        var result = await controller.Create(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ClientesController.GetById), createdResult.ActionName);
        Assert.IsType<ClientDto>(createdResult.Value);
        Assert.IsType<CreateClientCommand>(sender.LastRequest);
    }

    [Fact]
    public async Task GetById_WhenClientExists_ShouldReturnOk()
    {
        // Arrange
        var client = new ClientDto
        {
            Id = Guid.NewGuid(),
            Name = "Maria Silva",
            Cpf = "52998224725",
            Email = "maria.silva@email.com",
            CreatedAt = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)
        };
        var sender = new FakeSender
        {
            Response = client
        };
        var controller = new ClientesController(sender.Sender);

        // Act
        var result = await controller.GetById(client.Id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(client, okResult.Value);
        Assert.IsType<GetClientByIdQuery>(sender.LastRequest);
    }

    [Fact]
    public async Task GetById_WhenClientDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var sender = new FakeSender
        {
            Response = null
        };
        var controller = new ClientesController(sender.Sender);

        // Act
        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
