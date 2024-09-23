using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class LancamentoCommandHandler : IRequestHandler<LancamentoCommand, LancamentoResponse>
    {
        #region Dependencies

        private readonly ILauchClient _lauchClient;
        private readonly ILogger<LancamentoCommandHandler> _logger;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public LancamentoCommandHandler(ILauchClient lauchClient
                                      , ILogger<LancamentoCommandHandler> logger
                                      , RabbitMQPublisher rabbitMQPublisher)
        {
            _lauchClient = lauchClient;
            _logger = logger;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        #endregion END Dependencies

        public async Task<LancamentoResponse> Handle(LancamentoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                var grpcRequest = new LancamentoRequest
                {
                    Id = request.Id.ToString(),
                    Tipo = request.Tipo,
                    Valor = request.Valor,
                    Descricao = request.Descricao,
                    Data = request.Data,
                    ClienteId = request.ClienteId
                };

                var grpcResponse = await _lauchClient.RegistrarLancamentoAsync(grpcRequest, cancellationToken);

                var message = JsonConvert.SerializeObject(request);
                _rabbitMQPublisher.PublishMessage("queue_consolidacao_diaria", message);

                return new LancamentoResponse
                {
                    Id = grpcResponse.Id,
                    Sucesso = grpcResponse.Sucesso,
                    Mensagem = grpcResponse.Mensagem
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
