using Financeiro.CashFlow.Server;
using MediatR;

namespace Financeiro.CashFlow.Business.Commands
{
    public record DeletarLancamentoCommand(Guid Id) : IRequest<DeletarLancamentoResponse>;    
}
