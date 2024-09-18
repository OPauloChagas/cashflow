using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class AtualizarLancamentoCommandHandler : IRequestHandler<AtualizarLancamentoCommand, LancamentoResponse>
    {
        private readonly LancamentoAppDbContext _context;
        public AtualizarLancamentoCommandHandler(LancamentoAppDbContext context) => _context = context;
        public async Task<LancamentoResponse> Handle(AtualizarLancamentoCommand request, CancellationToken cancellationToken)
        {
            var lancamento = await _context.Lancamentos
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

            if (lancamento == null)
            {
                return new LancamentoResponse
                {
                    Sucesso = false,
                    Mensagem = "Lançamento não encontrado"
                };
            }

            var lancamentoAtualizado = lancamento with
            {
                Tipo = request.Tipo,
                Valor = request.Valor,
                Descricao = request.Descricao,
                Data = request.Data,
                ClienteId = request.ClienteId
            };

            _context.Lancamentos.Update(lancamentoAtualizado);
            await _context.SaveChangesAsync(cancellationToken);

            return new LancamentoResponse
            {
                Id = lancamentoAtualizado.Id.ToString(),
                Sucesso = true,
                Mensagem = "Lançamento atualizado com sucesso"
            };
        }
    }
}
