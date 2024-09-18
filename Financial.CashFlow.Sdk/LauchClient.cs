using Financeiro.CashFlow.DataModels;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Financial.CashFlow.Sdk
{
    public class LauchClient : ILauchClient
    {
        private readonly ILogger<LauchClient> _logger;
        private readonly LancamentoService.LancamentoServiceClient _lancamentoClient;
        private readonly LancamentoAppDbContext _context; 

        public LauchClient(
            LancamentoService.LancamentoServiceClient lancamentoClient,
            LancamentoAppDbContext context, // Injetar o contexto para simulação
            ILogger<LauchClient> logger)
        {
            _lancamentoClient = lancamentoClient;
            _context = context; // Para simulação com InMemory
            _logger = logger;
        }

        public async Task<LancamentoResponse> AtualizarLancamentoAsync(LancamentoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lancamentoClient.AtualizarLancamentoAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao atualizar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao atualizar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar lançamento: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<DeletarLancamentoResponse> DeletarLancamentoAsync(LancamentoIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lancamentoClient.DeletarLancamentoAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao deletar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao deletar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar lançamento: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<LancamentoResponse> RegistrarLancamentoAsync(LancamentoRequest request, CancellationToken cancellationToken)
        {
            var response = new LancamentoResponse();
            try
            {
                // Tenta registrar via gRPC
                response = await _lancamentoClient.RegistrarLancamentoAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao registrar lançamento via gRPC: {Message}", rpcEx.Message);

                // Simulação de fallback com InMemory caso o gRPC falhe
                _logger.LogInformation("Registrando o lançamento localmente no InMemory (simulação)");

                // Passar todos os parâmetros necessários, incluindo o 'Id'
                var lancamentoSimulado = new LancamentoDataModel(
                    Guid.NewGuid(), // Gera um novo Guid para o Id
                    request.Tipo,
                    request.Valor,
                    request.Descricao,
                    request.Data,
                    request.ClienteId
                );

                _context.Lancamentos.Add(lancamentoSimulado);
                await _context.SaveChangesAsync(cancellationToken);

                // Retorna uma resposta simulada de sucesso
                return new LancamentoResponse
                {
                    Id = lancamentoSimulado.Id.ToString(),
                    Sucesso = true,
                    Mensagem = "Lançamento registrado localmente (simulação)"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }

}
