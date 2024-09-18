using Financeiro.CashFlow.Server;
using Grpc.Core;

namespace Financial.CashFlow.Sdk
{
    public interface IGreeterService
    {
        Task<string> SayHelloAsync(string nome, CancellationToken cancellationToken);
    }

    public class GreeterGrpcService : IGreeterService
    {
        private readonly Greeter.GreeterClient grpcClient;

        public GreeterGrpcService(Greeter.GreeterClient grpcClient)
        {
            this.grpcClient = grpcClient;
        }

        public async Task<string> SayHelloAsync(string nome, CancellationToken cancellationToken)
        {
            try
            {
                var result = await grpcClient.SayHelloAsync(new HelloRequest { Name = nome }, null, null, cancellationToken);
                return result.Message;
            }
            catch (RpcException ex)
            {

                throw;
            }
        }
    }
}
