using Financeiro.CashFlow.Server;
using MediatR;

namespace Financeiro.CashFlow.Business.Commands
{
    public record DeleteLaunchCommand(Guid Id) : IRequest<DeleteLaunchResponse>;
}
