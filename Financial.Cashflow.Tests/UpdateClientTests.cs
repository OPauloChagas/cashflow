using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using static Financeiro.CashFlow.Server.LaunchRegisterService;

namespace Financial.Cashflow.Tests
{
    public class UpdateClientTests
    {
        private readonly Mock<LaunchRegisterServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public UpdateClientTests()
        {
            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LaunchRegisterServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AtualizarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LaunchRequest
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Debito",
                Value = 200.0,
                Description = "Compra",
                Date = "2024-09-21T00:00:00Z",
                ClientId = "Cliente456"
            };

            var response = new LaunchResponse
            {
                Id = request.Id,
                Success = true,
                Message = "Lançamento atualizado com sucesso!"
            };

            var asyncUnaryCall = new AsyncUnaryCall<LaunchResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.UpdateLaunchAsync(It.IsAny<LaunchRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.UpdateLaunchAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento atualizado com sucesso!", result.Message);
        }

        [Fact]
        public async Task AtualizarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LaunchRequest
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Debito",
                Value = 200.0,
                Description = "Compra",
                Date = "2024-09-21T00:00:00Z",
                ClientId = "Cliente456"
            };

            var asyncUnaryCall = new AsyncUnaryCall<LaunchResponse>(
                Task.FromException<LaunchResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.UpdateLaunchAsync(It.IsAny<LaunchRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.UpdateLaunchAsync(request, CancellationToken.None));
            Assert.Equal("Erro ao atualizar lançamento", ex.Message);
        }
    }
}
