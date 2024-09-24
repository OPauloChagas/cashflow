using Financeiro.CashFlow.Business;
using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Financial.Cashflow.Tests
{
    public class LancamentoCommandHandlerTests
    {
        private readonly Mock<ILauchClient> _mockGrpcClient;
        private readonly LancamentoCommandHandler _handler;
        private readonly Mock<ILogger<LancamentoCommandHandler>> _mockLogger;
        private readonly Mock<RabbitMQPublisher> _mockRabbitMQPublisher;

        public LancamentoCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<LancamentoCommandHandler>>();
            _mockGrpcClient = new Mock<ILauchClient>();
            _mockRabbitMQPublisher = new Mock<RabbitMQPublisher>();

            _mockRabbitMQPublisher
              .Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>()))
              .Verifiable();

            _handler = new LancamentoCommandHandler(_mockGrpcClient.Object, _mockLogger.Object, _mockRabbitMQPublisher.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new LancamentoCommand(
                Guid.NewGuid(),
                "Credito",
                100.0,
                "Pagamento",
                "2023-09-21",
                "Cliente123"
            );

            var grpcResponse = new LancamentoResponse { Sucesso = true };

            _mockGrpcClient.Setup(client => client.RegistrarLancamentoAsync(It.IsAny<LancamentoRequest>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            _mockRabbitMQPublisher.Verify(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new LancamentoCommand(
                Guid.NewGuid(),
                "Credito",
                100.0,
                "Pagamento",
                "2023-09-21",
                "Cliente123"
            );
            _mockGrpcClient.Setup(client => client.RegistrarLancamentoAsync(It.IsAny<LancamentoRequest>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);
        }

    }
}
