using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class DeleteLaunchCommandHandler : IRequestHandler<DeleteLaunchCommand, DeleteLaunchResponse>
    {
        #region Dependencies

        private readonly ILaunchClient _launchClient;
        private readonly ILogger<DeleteLaunchCommandHandler> _logger;

        public DeleteLaunchCommandHandler(ILaunchClient launchClient
                                      , ILogger<DeleteLaunchCommandHandler> logger)
        {
            _launchClient = launchClient;
            _logger = logger;
        }

        #endregion END Dependencies

        public async Task<DeleteLaunchResponse> Handle(DeleteLaunchCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var grpRequestExcluir = new LaunchIdRequest{ Id = request.Id.ToString() };
                var grpcExcluir = await _launchClient.DeleteLaunchAsync(grpRequestExcluir, cancellationToken);

                return new DeleteLaunchResponse
                {
                    Success = grpcExcluir.Success,
                    Message = grpcExcluir.Message
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro na comunicação com o serviço gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao comunicar com o serviço gRPC", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }

}
