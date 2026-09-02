using CadastroClientes.Api.Controllers;
using CadastroClientes.Application.Commands;
using CadastroClientes.Application.DTOs;
using CadastroClientes.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;

namespace CadastroClientes.UnitTests.Api;

public class ClientesControllerTests
{
    [Fact]
    public async Task Create_WithValidCommand_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var response = new ClientDto
        {
            Id = Guid.NewGuid(),
            Name = "Maria Silva",
            Cpf = "52998224725",
            Email = "maria.silva@email.com",
            CreatedAt = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)
        };
        var sender = new Mock<ISender>();
        sender
            .Setup(item => item.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        var controller = new ClientesController(sender.Object);
        var command = new CreateClientCommand
        {
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            Email = "maria.silva@email.com",
            Score = 500
        };

        // Act
        var result = await controller.Create(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ClientesController.GetById), createdResult.ActionName);
        Assert.IsType<ClientDto>(createdResult.Value);
        sender.Verify(
            item => item.Send(
                It.Is<CreateClientCommand>(request =>
                    request.Name == command.Name &&
                    request.Cpf == command.Cpf &&
                    request.Email == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
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
        var sender = new Mock<ISender>();
        sender
            .Setup(item => item.Send(It.IsAny<GetClientByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        var controller = new ClientesController(sender.Object);

        // Act
        var result = await controller.GetById(client.Id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(client, okResult.Value);
        sender.Verify(
            item => item.Send(
                It.Is<GetClientByIdQuery>(query => query.Id == client.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetById_WhenClientDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var sender = new Mock<ISender>();
        sender
            .Setup(item => item.Send(It.IsAny<GetClientByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientDto?)null);
        var controller = new ClientesController(sender.Object);

        // Act
        var result = await controller.GetById(clientId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        sender.Verify(
            item => item.Send(
                It.Is<GetClientByIdQuery>(query => query.Id == clientId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
