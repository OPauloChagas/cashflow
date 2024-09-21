using Financeiro.CashFlow.DataModels;
using Financeiro.CashFlow.DataModels.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;


namespace Financeiro.CashFlow.Server.Services
{
    public class LauchService : LancamentoService.LancamentoServiceBase
    {
        private readonly LancamentoAppDbContext _context;
        private readonly ILogger<LauchService> _logger; // Adicionar logger para capturar erros

        public LauchService(LancamentoAppDbContext context, ILogger<LauchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<LancamentoResponse> RegistrarLancamento(LancamentoRequest request, ServerCallContext context)
        {
            try
            {
                // Cria um novo lançamento
                var lancamento = new LancamentoDataModel(
                    Guid.Parse(request.Id),
                    request.Tipo,
                    request.Valor,
                    request.Descricao,
                    request.Data,
                    request.ClienteId
                );

                // Tenta salvar no contexto InMemory
                _context.Lancamentos.Add(lancamento);
                await _context.SaveChangesAsync();

                // Retorna a resposta de sucesso
                return new LancamentoResponse
                {
                    Id = lancamento.Id.ToString(),
                    Sucesso = true,
                    Mensagem = "Lançamento registrado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                // Logar a exceção para facilitar o diagnóstico
                _logger.LogError(ex, "Erro ao registrar lançamento: {Message}", ex.Message);

                // Retornar uma resposta de erro apropriada
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao registrar o lançamento: {ex.Message}"));
            }
        }

        public override async Task<LancamentoResponse> AtualizarLancamento(LancamentoRequest request, ServerCallContext context)
        {
            try
            {
                // Tenta buscar o lançamento existente no contexto InMemory
                var lancamentoExistente = await _context.Lancamentos
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id));

                if (lancamentoExistente == null)
                {
                    return new LancamentoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Lançamento não encontrado"
                    };
                }

                // Atualiza os campos do lançamento
                var lancamentoAtualizado = lancamentoExistente with
                {
                    Tipo = request.Tipo,
                    Valor = request.Valor,
                    Descricao = request.Descricao,
                    Data = request.Data,
                    ClienteId = request.ClienteId
                };

                // Salva as alterações no contexto
                _context.Lancamentos.Update(lancamentoAtualizado);
                await _context.SaveChangesAsync();

                // Retorna a resposta de sucesso
                return new LancamentoResponse
                {
                    Id = lancamentoExistente.Id.ToString(),
                    Sucesso = true,
                    Mensagem = "Lançamento atualizado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar lançamento: {Message}", ex.Message);

                // Retorna uma resposta de erro apropriada
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao atualizar o lançamento: {ex.Message}"));
            }
        }

        public override async Task<DeletarLancamentoResponse> DeletarLancamento(LancamentoIdRequest request, ServerCallContext context)
        {
            try
            {
                // Tenta buscar o lançamento existente no contexto InMemory
                var lancamentoExistente = await _context.Lancamentos.FindAsync(Guid.Parse(request.Id));

                if (lancamentoExistente == null)
                {
                    return new DeletarLancamentoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Lançamento não encontrado"
                    };
                }

                // Remove o lançamento
                _context.Lancamentos.Remove(lancamentoExistente);
                await _context.SaveChangesAsync();

                // Retorna a resposta de sucesso
                return new DeletarLancamentoResponse
                {
                    Sucesso = true,
                    Mensagem = "Lançamento deletado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar lançamento: {Message}", ex.Message);

                // Retorna uma resposta de erro apropriada
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao deletar o lançamento: {ex.Message}"));
            }
        }

    }
}
