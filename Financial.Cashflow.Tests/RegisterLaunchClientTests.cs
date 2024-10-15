using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using static Financeiro.CashFlow.Server.LaunchRegisterService;

namespace Financial.Cashflow.Tests
{
    public class RegisterLaunchClientTests
    {
        private readonly Mock<LaunchRegisterServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public RegisterLaunchClientTests()
        {
            
            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LaunchRegisterServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LaunchRequest
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Credito",
                Value = 100.0,
                Description = "Pagamento",
                Date = "2024-09-14T00:00:00Z",
                ClientId = "Cliente123"
            };

            var response = new LaunchResponse
            {
                Id = request.Id,
                Success = true,
                Message = "Lançamento registrado com sucesso!"
            };

            // Simulando uma chamada gRPC com sucesso
            var asyncUnaryCall = new AsyncUnaryCall<LaunchResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.RegisterLaunchAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento registrado com sucesso!", result.Message);
        }

        [Fact]
        public async Task RegistrarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LaunchRequest
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Credito",
                Value = 100.0,
                Description = "Pagamento",
                Date = "2024-09-14T00:00:00Z",
                ClientId = "Cliente123"
            };

            // Configurando o mock do AsyncUnaryCall para lançar uma exceção RpcException
            var asyncUnaryCall = new AsyncUnaryCall<LaunchResponse>(
                Task.FromException<LaunchResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.RegisterLaunchAsync(It.IsAny<LaunchRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.RegisterLaunchAsync(request, CancellationToken.None));

            // Verificando se a mensagem da exceção está correta
            Assert.Equal("Erro ao registrar lançamento", ex.Message);
        }
    }

}

