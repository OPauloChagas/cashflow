using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class AtualizarLancamentoCommandHandler : IRequestHandler<AtualizarLancamentoCommand, LancamentoResponse>
    {
        #region Dependencies

        private readonly ILauchClient _lauchClient;
        private readonly ILogger<AtualizarLancamentoCommandHandler> _logger;

        public AtualizarLancamentoCommandHandler(ILauchClient lauchClient
                                      , ILogger<AtualizarLancamentoCommandHandler> logger)
        {
            _lauchClient = lauchClient;
            _logger = logger;
        }

        #endregion END Dependencies
                
        public async Task<LancamentoResponse> Handle(AtualizarLancamentoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lancamentoAtualizar = new LancamentoRequest
                {
                    Id = request.Id.ToString(),
                    Tipo = request.Tipo,
                    Valor = request.Valor,
                    Descricao = request.Descricao,
                    Data = request.Data,
                    ClienteId = request.ClienteId
                };

                var grpcAtualizar = await _lauchClient.AtualizarLancamentoAsync(lancamentoAtualizar, cancellationToken);

                return new LancamentoResponse
                {
                    Id = grpcAtualizar.Id.ToString(),
                    Sucesso = true,
                    Mensagem = "Lançamento atualizado com sucesso"
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
