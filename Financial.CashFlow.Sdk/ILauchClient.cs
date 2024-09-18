using Financeiro.CashFlow.Server;

namespace Financial.CashFlow.Sdk
{
    public interface ILauchClient
    {
        Task<LancamentoResponse> RegistrarLancamentoAsync(LancamentoRequest request, CancellationToken cancellationToken);
        Task<LancamentoResponse> AtualizarLancamentoAsync(LancamentoRequest request, CancellationToken cancellationToken);
        Task<DeletarLancamentoResponse> DeletarLancamentoAsync(LancamentoIdRequest request, CancellationToken cancellationToken);
    }
}
