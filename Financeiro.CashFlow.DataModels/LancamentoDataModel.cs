using MongoDB.Bson.Serialization.Attributes;

namespace Financeiro.CashFlow.DataModels
{
    public record LancamentoDataModel
    {
        [BsonId]
        public Guid Id { get; init; }
        public string? Tipo { get; init; }
        public double Valor { get; init; }
        public string? Descricao { get; init; }
        public string? Data { get; init; }
        public string? ClienteId { get; init; }

        public static LancamentoDataModel Create(Guid lancamentoId, string tipoLancamento, double valor, string descricao, string data, string clienteId)
        {
            return new LancamentoDataModel
            {
                Id = lancamentoId,
                Tipo = tipoLancamento,
                Valor = valor,
                Descricao = descricao,
                Data = data,
                ClienteId = clienteId
            };
        }
    }


}
