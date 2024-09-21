using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using static Financeiro.CashFlow.Server.LancamentoService;

namespace Financial.Cashflow.Tests
{
    public class AtualizarClientTests
    {
        private readonly Mock<LancamentoServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public AtualizarClientTests()
        {
            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LancamentoServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AtualizarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LancamentoRequest
            {
                Id = Guid.NewGuid().ToString(),
                Tipo = "Debito",
                Valor = 200.0,
                Descricao = "Compra",
                Data = "2024-09-21T00:00:00Z",
                ClienteId = "Cliente456"
            };

            var response = new LancamentoResponse
            {
                Id = request.Id,
                Sucesso = true,
                Mensagem = "Lançamento atualizado com sucesso!"
            };

            var asyncUnaryCall = new AsyncUnaryCall<LancamentoResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.AtualizarLancamentoAsync(It.IsAny<LancamentoRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.AtualizarLancamentoAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal("Lançamento atualizado com sucesso!", result.Mensagem);
        }

        [Fact]
        public async Task AtualizarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LancamentoRequest
            {
                Id = Guid.NewGuid().ToString(),
                Tipo = "Debito",
                Valor = 200.0,
                Descricao = "Compra",
                Data = "2024-09-21T00:00:00Z",
                ClienteId = "Cliente456"
            };

            var asyncUnaryCall = new AsyncUnaryCall<LancamentoResponse>(
                Task.FromException<LancamentoResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.AtualizarLancamentoAsync(It.IsAny<LancamentoRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.AtualizarLancamentoAsync(request, CancellationToken.None));
            Assert.Equal("Erro ao atualizar lançamento", ex.Message);
        }
    }
}
