using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class DeletarLancamentoCommandHandler : IRequestHandler<DeletarLancamentoCommand, DeletarLancamentoResponse>
    {
        private readonly ILauchClient _lauchClient;
        private readonly ILogger<DeletarLancamentoCommandHandler> _logger;

        public DeletarLancamentoCommandHandler(ILauchClient lauchClient
                                      , ILogger<DeletarLancamentoCommandHandler> logger)
        {
            _lauchClient = lauchClient;
            _logger = logger;
        }
        public async Task<DeletarLancamentoResponse> Handle(DeletarLancamentoCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var grpRequestExcluir = new LancamentoIdRequest{ Id = request.Id.ToString() };
                var grpcExcluir = await _lauchClient.DeletarLancamentoAsync(grpRequestExcluir, cancellationToken);

                return new DeletarLancamentoResponse
                {
                    Sucesso = grpcExcluir.Sucesso,
                    Mensagem = grpcExcluir.Mensagem
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
