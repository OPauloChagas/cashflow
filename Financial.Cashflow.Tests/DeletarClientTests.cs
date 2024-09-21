using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using static Financeiro.CashFlow.Server.LancamentoService;

namespace Financial.Cashflow.Tests
{
    public class DeletarClientTests
    {

        private readonly Mock<LancamentoServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public DeletarClientTests()
        {
            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LancamentoServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeletarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LancamentoIdRequest
            {
                Id = Guid.NewGuid().ToString()
            };

            var response = new DeletarLancamentoResponse
            {
                Sucesso = true,
                Mensagem = "Lançamento deletado com sucesso!"
            };

            var asyncUnaryCall = new AsyncUnaryCall<DeletarLancamentoResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.DeletarLancamentoAsync(It.IsAny<LancamentoIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.DeletarLancamentoAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal("Lançamento deletado com sucesso!", result.Mensagem);
        }

        [Fact]
        public async Task DeletarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LancamentoIdRequest
            {
                Id = Guid.NewGuid().ToString()
            };

            var asyncUnaryCall = new AsyncUnaryCall<DeletarLancamentoResponse>(
                Task.FromException<DeletarLancamentoResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.DeletarLancamentoAsync(It.IsAny<LancamentoIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.DeletarLancamentoAsync(request, CancellationToken.None));
            Assert.Equal("Erro ao deletar lançamento", ex.Message);
        }

    }
}
