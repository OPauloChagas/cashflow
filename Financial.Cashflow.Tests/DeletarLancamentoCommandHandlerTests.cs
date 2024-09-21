using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Financial.Cashflow.Tests
{
    public class DeletarLancamentoCommandHandlerTests
    {
        private readonly Mock<ILauchClient> _mockGrpcClient;
        private readonly DeletarLancamentoCommandHandler _handler;
        private readonly Mock<ILogger<DeletarLancamentoCommandHandler>> _mockLogger;

        public DeletarLancamentoCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<DeletarLancamentoCommandHandler>>();
            _mockGrpcClient = new Mock<ILauchClient>();
            _handler = new DeletarLancamentoCommandHandler(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_DeletarLancamentoGrpcServiceReturnsSuccess_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new DeletarLancamentoCommand(Guid.NewGuid()); 

            var grpcResponse = new DeletarLancamentoResponse
            {
                Sucesso = true,
                Mensagem = "Lançamento deletado com sucesso!"
            };

            // Simulando a chamada gRPC com sucesso
            _mockGrpcClient
                .Setup(client => client.DeletarLancamentoAsync(It.IsAny<LancamentoIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grpcResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal("Lançamento deletado com sucesso!", result.Mensagem);
        }

        [Fact]
        public async Task Handle_DeletarLancamentoGrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var command = new DeletarLancamentoCommand(Guid.NewGuid()); 

            // Simulando a exceção RpcException
            _mockGrpcClient
                .Setup(client => client.DeletarLancamentoAsync(It.IsAny<LancamentoIdRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "gRPC error")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));

            // Verificando se a exceção correta foi lançada
            Assert.Equal("Erro ao comunicar com o serviço gRPC", ex.Message);            
        }
    }
}
