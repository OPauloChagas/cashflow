using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class UpdateLaunchCommandHandler : IRequestHandler<UpdateLaunchCommand, LaunchResponse>
    {
        #region Dependencies

        private readonly ILaunchClient _lauchClient;
        private readonly ILogger<UpdateLaunchCommandHandler> _logger;

        public UpdateLaunchCommandHandler(ILaunchClient launchClient
                                      , ILogger<UpdateLaunchCommandHandler> logger)
        {
            _lauchClient = launchClient;
            _logger = logger;
        }

        #endregion END Dependencies
                
        public async Task<LaunchResponse> Handle(UpdateLaunchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lancamentoAtualizar = new LaunchRequest
                {
                    Id = request.Id.ToString(),
                    Type = request.Tipo,
                    Value = request.Valor,
                    Description = request.Descricao,
                    Date = request.Data,
                    ClientId = request.ClienteId
                };

                var grpcAtualizar = await _lauchClient.UpdateLaunchAsync(lancamentoAtualizar, cancellationToken);

                return new LaunchResponse
                {
                    Id = grpcAtualizar.Id.ToString(),
                    Success = true,
                    Message = "Lançamento atualizado com sucesso"
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro na comunicação com o serviço gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao comunicar com o serviço gRPC", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }
}
