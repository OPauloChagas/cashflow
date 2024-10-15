using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Financial.Cashflow.Tests
{
    public class UpdateLaunchCommandHandlerTests
    {
        private readonly Mock<ILaunchClient> _mockGrpcClient;
        private readonly UpdateLaunchCommandHandler _handler;
        private readonly Mock<ILogger<UpdateLaunchCommandHandler>> _mockLogger;

        public UpdateLaunchCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<UpdateLaunchCommandHandler>>();
            _mockGrpcClient = new Mock<ILaunchClient>();
            _handler = new UpdateLaunchCommandHandler(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_AtualizarLancamentoGrpcServiceReturnsSuccess_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new UpdateLaunchCommand
            (
                Guid.NewGuid(),
                "Debito",
                200.0,
                "Compra",
                "2024-09-21T00:00:00Z",
                "Cliente456"
            );

            var grpcResponse = new LaunchResponse
            {
                Id = command.Id.ToString(),
                Success = true,
                Message = "Lançamento atualizado com sucesso!"
            };

            // Simulando a chamada gRPC com sucesso
            _mockGrpcClient
                .Setup(client => client.UpdateLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento atualizado com sucesso", result.Message);
        }

        [Fact]
        public async Task Handle_AtualizarLancamentoGrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new UpdateLaunchCommand
            (
                 Guid.NewGuid(),
                "Debito",
                200.0,
                 "Compra",
                 "2024-09-21T00:00:00Z",
                "Cliente456"
            );

            // Simulando a exceção RpcException
            _mockGrpcClient
                .Setup(client => client.UpdateLaunchAsync(It.IsAny<LaunchRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            // Verificando se a exceção correta foi lançada
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);

        }
    }
}
