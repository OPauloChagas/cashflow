using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using static Financeiro.CashFlow.Server.LaunchRegisterService;

namespace Financial.Cashflow.Tests
{
    public class DeleteClientTests
    {

        private readonly Mock<LaunchRegisterServiceClient> _mockGrpcClient;
        private readonly LauchClient _client;
        private readonly Mock<ILogger<LauchClient>> _mockLogger;

        public DeleteClientTests()
        {
            _mockLogger = new Mock<ILogger<LauchClient>>();
            _mockGrpcClient = new Mock<LaunchRegisterServiceClient>();
            _client = new LauchClient(_mockGrpcClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeletarLancamentoAsync_GrpcServiceReturnsValidResponse_ReturnsSuccess()
        {
            // Arrange
            var request = new LaunchIdRequest
            {
                Id = Guid.NewGuid().ToString()
            };

            var response = new DeleteLaunchResponse
            {
                Success = true,
                Message = "Lançamento deletado com sucesso!"
            };

            var asyncUnaryCall = new AsyncUnaryCall<DeleteLaunchResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.DeleteLaunchAsync(It.IsAny<LaunchIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act
            var result = await _client.DeleteLaunchAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Lançamento deletado com sucesso!", result.Message);
        }

        [Fact]
        public async Task DeletarLancamentoAsync_GrpcServiceThrowsRpcException_ThrowsApplicationException()
        {
            // Arrange
            var request = new LaunchIdRequest
            {
                Id = Guid.NewGuid().ToString()
            };

            var asyncUnaryCall = new AsyncUnaryCall<DeleteLaunchResponse>(
                Task.FromException<DeleteLaunchResponse>(new RpcException(new Status(StatusCode.Internal, "gRPC error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }
            );

            _mockGrpcClient
                .Setup(client => client.DeleteLaunchAsync(It.IsAny<LaunchIdRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.DeleteLaunchAsync(request, CancellationToken.None));
            Assert.Equal("Erro ao deletar lançamento", ex.Message);
        }

    }
}
