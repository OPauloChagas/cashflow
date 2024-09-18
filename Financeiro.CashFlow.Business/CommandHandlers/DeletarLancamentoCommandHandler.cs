using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.CashFlow.Business.CommandHandlers
{
    public class DeletarLancamentoCommandHandler : IRequestHandler<DeletarLancamentoCommand, DeletarLancamentoResponse>
    {
        private readonly LancamentoAppDbContext _context;
        public DeletarLancamentoCommandHandler(LancamentoAppDbContext context) => _context = context;        
        public async Task<DeletarLancamentoResponse> Handle(DeletarLancamentoCommand request, CancellationToken cancellationToken)
        {
            var lancamento = await _context.Lancamentos
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

            if (lancamento == null)
            {
                return new DeletarLancamentoResponse
                {
                    Sucesso = false,
                    Mensagem = "Lançamento não encontrado"
                };
            }

            _context.Lancamentos.Remove(lancamento);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeletarLancamentoResponse
            {
                Sucesso = true,
                Mensagem = "Lançamento deletado com sucesso"
            };
        }
    }

}
