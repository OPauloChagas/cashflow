using Grpc.Core;

namespace Financeiro.CashFlow.Server.Services
{
    public class LauchService : LancamentoService.LancamentoServiceBase
    {
        public override Task<LancamentoResponse> RegistrarLancamento(LancamentoRequest request, ServerCallContext context)
        {
            var response = new LancamentoResponse
            {
                Id = Guid.NewGuid().ToString(),
                Sucesso = true,
                Mensagem = "Lançamento registrado com sucesso!"
            };

            return Task.FromResult(response);
        }
    }
}
