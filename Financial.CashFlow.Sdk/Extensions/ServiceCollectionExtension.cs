using Financeiro.CashFlow.Server;
using Microsoft.Extensions.DependencyInjection;


namespace Financial.CashFlow.Sdk.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddGrpcSdk(this IServiceCollection services)
        {
            services.AddGrpcClient<LaunchRegisterService.LaunchRegisterServiceClient>(client =>
            {
                client.Address = new Uri("https://localhost:7191");
            });

            services.AddScoped<ILaunchClient, LauchClient>();
        }
    }
}
