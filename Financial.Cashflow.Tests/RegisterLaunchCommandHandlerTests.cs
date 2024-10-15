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
    public class RegisterLaunchCommandHandlerTests
    {
        private readonly Mock<ILaunchClient> _mockGrpcClient;
        private readonly RegisterLaunchCommandHandler _handler;
        private readonly Mock<ILogger<RegisterLaunchCommandHandler>> _mockLogger;
        private readonly Mock<RabbitMQPublisher> _mockRabbitMQPublisher;

        public RegisterLaunchCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<RegisterLaunchCommandHandler>>();
            _mockGrpcClient = new Mock<ILaunchClient>();
            _mockRabbitMQPublisher = new Mock<RabbitMQPublisher>();

            _mockRabbitMQPublisher
              .Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>()))
              .Verifiable();

            _handler = new RegisterLaunchCommandHandler(_mockGrpcClient.Object, _mockLogger.Object, _mockRabbitMQPublisher.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new RegisterLaunchCommand(
                Guid.NewGuid(),
                "Credito",
                100.0,
                "Pagamento",
                "2023-09-21",
                "Cliente123"
            );

            var grpcResponse = new LaunchResponse { Success = true };

            _mockGrpcClient.Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockRabbitMQPublisher.Verify(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new RegisterLaunchCommand(
                Guid.NewGuid(),
                "Credito",
                100.0,
                "Pagamento",
                "2023-09-21",
                "Cliente123"
            );
            _mockGrpcClient.Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);
        }

    }
}
