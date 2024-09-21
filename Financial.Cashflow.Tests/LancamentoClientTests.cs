using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using static Financeiro.CashFlow.Server.LancamentoService;

namespace Financial.Cashflow.Tests
{
    public class LancamentoClientTests
    {
        private readonly Mock<LancamentoServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public LancamentoClientTests()
        {
            
            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LancamentoServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LancamentoRequest
            {
                Id = Guid.NewGuid().ToString(),
                Tipo = "Credito",
                Valor = 100.0,
                Descricao = "Pagamento",
                Data = "2024-09-14T00:00:00Z",
                ClienteId = "Cliente123"
            };

            var response = new LancamentoResponse
            {
                Id = request.Id,
                Sucesso = true,
                Mensagem = "Lançamento registrado com sucesso!"
            };

            // Simulando uma chamada gRPC com sucesso
            var asyncUnaryCall = new AsyncUnaryCall<LancamentoResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.RegistrarLancamentoAsync(It.IsAny<LancamentoRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.RegistrarLancamentoAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal("Lançamento registrado com sucesso!", result.Mensagem);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LancamentoRequest
            {
                Id = Guid.NewGuid().ToString(),
                Tipo = "Credito",
                Valor = 100.0,
                Descricao = "Pagamento",
                Data = "2024-09-14T00:00:00Z",
                ClienteId = "Cliente123"
            };

            // Configurando o mock do AsyncUnaryCall para lançar uma exceção RpcException
            var asyncUnaryCall = new AsyncUnaryCall<LancamentoResponse>(
                Task.FromException<LancamentoResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.RegistrarLancamentoAsync(It.IsAny<LancamentoRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.RegistrarLancamentoAsync(request, CancellationToken.None));

            // Verificando se a mensagem da exceção está correta
            Assert.Equal("Erro ao registrar lançamento", ex.Message);
        }
    }

}

