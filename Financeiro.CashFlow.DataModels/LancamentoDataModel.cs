namespace Financeiro.CashFlow.DataModels
{
    public record LancamentoDataModel(Guid Id,
                                     string Tipo,
                                     double Valor,
                                     string Descricao,
                                     string Data,
                                     string ClienteId)
    {

        public static LancamentoDataModel Create(Guid lancamentoId, string tipoLancamento, double valor, string descricao, string data, string clienteId)
        {
            return new LancamentoDataModel(lancamentoId, tipoLancamento, valor, descricao, data, clienteId);
        }
    }

}
