using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Financial.Cashflow.Tests
{
    public class DeleteLaunchCommandHandlerTests
    {
        private readonly Mock<ILaunchClient> _mockGrpcClient;
        private readonly DeleteLaunchCommandHandler _handler;
        private readonly Mock<ILogger<DeleteLaunchCommandHandler>> _mockLogger;

        public DeleteLaunchCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<DeleteLaunchCommandHandler>>();
            _mockGrpcClient = new Mock<ILaunchClient>();
            _handler = new DeleteLaunchCommandHandler(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_DeletarLancamentoGrpcServiceReturnsSuccess_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new DeleteLaunchCommand(Guid.NewGuid()); 

            var grpcResponse = new DeleteLaunchResponse
            {
                Success = true,
                Message = "Lançamento deletado com sucesso!"
            };

            // Simulando a chamada gRPC com sucesso
            _mockGrpcClient
                .Setup(client => client.DeleteLaunchAsync(It.IsAny<LaunchIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento deletado com sucesso!", result.Message);
        }

        [Fact]
        public async Task Handle_DeletarLancamentoGrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new DeleteLaunchCommand(Guid.NewGuid()); 

            // Simulando a exceção RpcException
            _mockGrpcClient
                .Setup(client => client.DeleteLaunchAsync(It.IsAny<LaunchIdRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            // Verificando se a exceção correta foi lançada
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);            
        }
    }
}
