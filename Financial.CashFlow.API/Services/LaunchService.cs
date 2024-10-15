using Grpc.Core;

namespace Financeiro.CashFlow.Server.Services
{
    public class LaunchService : LaunchRegisterService.LaunchRegisterServiceBase
    {
        public override Task<LaunchResponse> RegisterLaunch(LaunchRequest request, ServerCallContext context)
        {
            var response = new LaunchResponse
            {
                Id = Guid.NewGuid().ToString(),
                Success = true,
                Message = "Lançamento registrado com sucesso!"
            };

            return Task.FromResult(response);
        }
    }
}
