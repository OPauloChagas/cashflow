using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class RegisterLaunchCommandHandler : IRequestHandler<RegisterLaunchCommand, LaunchResponse>
    {
        #region Dependencies

        private readonly ILaunchClient _lauchClient;
        private readonly ILogger<RegisterLaunchCommandHandler> _logger;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public RegisterLaunchCommandHandler(ILaunchClient lauchClient
                                      , ILogger<RegisterLaunchCommandHandler> logger
                                      , RabbitMQPublisher rabbitMQPublisher)
        {
            _lauchClient = lauchClient;
            _logger = logger;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        #endregion END Dependencies

        public async Task<LaunchResponse> Handle(RegisterLaunchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                var grpcRequest = new LaunchRequest
                {
                    Id = request.Id.ToString(),
                    Type = request.Tipo,
                    Value = request.Valor,
                    Description = request.Descricao,
                    Date = request.Data,
                    ClientId = request.ClienteId
                };

                var grpcResponse = await _lauchClient.RegisterLaunchAsync(grpcRequest, cancellationToken);

                var message = JsonConvert.SerializeObject(request);
                _rabbitMQPublisher.PublishMessage("queue_consolidacao_diaria", message);

                return new LaunchResponse
                {
                    Id = grpcResponse.Id,
                    Success = grpcResponse.Success,
                    Message = grpcResponse.Message
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro na comunicação com o serviço gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao comunicar com o serviço gRPC", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }
}
