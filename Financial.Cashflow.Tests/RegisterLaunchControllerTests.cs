using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Financial.Cashflow.Tests
{
    public class RegisterLaunchControllerTests
    {
        private readonly LancamentoController _controller;
        private readonly Mock<ISender> _mockSender;

        public RegisterLaunchControllerTests()
        {
            _mockSender = new Mock<ISender>();
            _controller = new LancamentoController(_mockSender.Object);
        }

        [Fact]
        public async Task CriarLancamento_CommandIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CriarLancamento(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("O comando de lançamento não pode ser nulo.", badRequestResult.Value);
        }

        [Fact]
        public async Task CriarLancamento_ValidCommand_ReturnsOkResult()
        {
            // Arrange
            var command = new RegisterLaunchCommand(
                Guid.NewGuid(),
                "Credito",
                100.0,
                "Vendas",
                "2023-09-21",
                "5052"
            );
            var response = new LaunchResponse { Success = true };
            _mockSender.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.CriarLancamento(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task CriarLancamento_CommandFails_ReturnsBadRequest()
        {
            // Arrange
            var command = new RegisterLaunchCommand(
                Guid.NewGuid(),
                "Debito",
                1000.0,
                "Fornecedores",
                "2023-09-22",
                "8681"
            );
            var response = new LaunchResponse { Success = false, Message = "Erro ao processar." };
            _mockSender.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.CriarLancamento(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro ao processar.", badRequestResult.Value);
        }

        [Fact]
        public async Task AtualizarLancamento_ValidCommand_ReturnsOkResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateLaunchCommand
            (
                 id,
                 "Debito",
                 100.0,
                 "Compra",
                 "2024-09-21T00:00:00Z",
                 "Cliente123"
            );

            var response = new LaunchResponse { Success = true, Message = "Lançamento atualizado com sucesso!" };

            _mockSender.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.AtualizarLancamento(id, command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            Assert.Equal(response.Message, okResult.Value);        
        }


        [Fact]
        public async Task AtualizarLancamento_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateLaunchCommand
            (
                Guid.NewGuid(), // ID diferente do 'id'
                "Credito",
                200.0,
                "Pagamento",
                "2024-09-21T00:00:00Z",
                "Cliente123"
            );

            // Act
            var result = await _controller.AtualizarLancamento(id, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID na URL não corresponde ao ID no corpo da requisição.", badRequestResult.Value);
        }


        [Fact]
        public async Task AtualizarLancamento_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateLaunchCommand
            (
                id,
                "Credito",
                200.0,
                "Pagamento",
                "2024-09-21T00:00:00Z",
                "Cliente123"
            );

            _mockSender.Setup(s => s.Send(It.IsAny<UpdateLaunchCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((LaunchResponse)null);

            // Act
            var result = await _controller.AtualizarLancamento(id, command);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); // Verifica se é um NotFound
            Assert.Equal("Lançamento não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task AtualizarLancamento_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateLaunchCommand
            (
                id,
                "Credito",
                200.0,
                "Pagamento",
                "2024-09-21T00:00:00Z",
                "Cliente123"
            );

            _mockSender.Setup(s => s.Send(It.IsAny<UpdateLaunchCommand>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new ApplicationException("Erro ao atualizar o lançamento"));

            // Act
            var result = await _controller.AtualizarLancamento(id, command);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro ao atualizar o lançamento: Erro ao atualizar o lançamento", statusCodeResult.Value);
        }



        [Fact]
        public async Task DeletarLancamento_ValidId_ReturnsOkResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteLaunchCommand(id);
            var response = new DeleteLaunchResponse { Success = true, Message = "Lançamento deletado com sucesso!" };

            _mockSender.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.DeletarLancamento(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task DeletarLancamento_EmptyId_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.Empty;

            // Act
            var result = await _controller.DeletarLancamento(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id do lançamento não pode ser vazio.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeletarLancamento_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteLaunchCommand(id);
            var response = new DeleteLaunchResponse { Success = false, Message = "Lançamento não encontrado." };

            _mockSender.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.DeletarLancamento(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Lançamento não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeletarLancamento_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteLaunchCommand(id);

            _mockSender.Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("Erro ao deletar o lançamento"));

            // Act
            var result = await _controller.DeletarLancamento(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro ao deletar o lançamento: Erro ao deletar o lançamento", statusCodeResult.Value);
        }
    }
}
