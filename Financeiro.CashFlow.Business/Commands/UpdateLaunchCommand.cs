using Financeiro.CashFlow.Server;
using MediatR;

namespace Financeiro.CashFlow.Business.Commands
{
    public record UpdateLaunchCommand(Guid Id, string Tipo, double Valor, string Descricao, string Data, string ClienteId) : IRequest<LaunchResponse>;

}
