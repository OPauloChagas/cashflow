using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using Financial.CashFlow.Sdk;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class LancamentoCommandHandler : IRequestHandler<LancamentoCommand, LancamentoResponse>
    {
        private readonly ILauchClient _lauchClient;
        private readonly ILogger<LancamentoCommandHandler> _logger;

        public LancamentoCommandHandler(ILauchClient lauchClient
                                      , ILogger<LancamentoCommandHandler> logger)
        {
            _lauchClient = lauchClient;
            _logger = logger;
        }
        public async Task<LancamentoResponse> Handle(LancamentoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Mapeia o comando para o gRPC LancamentoRequest
                var grpcRequest = new LancamentoRequest
                {
                    Id = request.Id.ToString(),
                    Tipo = request.Tipo,
                    Valor = request.Valor,
                    Descricao = request.Descricao,
                    Data = request.Data,
                    ClienteId = request.ClienteId
                };

                // Faz a chamada gRPC usando o LauchClient para registrar o lançamento
                var grpcResponse = await _lauchClient.RegistrarLancamentoAsync(grpcRequest, cancellationToken);


                // Retorna a resposta do gRPC como resultado do CommandHandler
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
