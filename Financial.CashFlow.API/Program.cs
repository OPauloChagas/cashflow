using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server.Services;
using Financial.CashFlow.Sdk.Extensions;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Adiciona os controladores (Web API)
builder.Services.AddControllers();


// Configuração de DbContext com InMemory
builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
    options.UseInMemoryDatabase("LancamentoDb"));


// Configuração do MediatR para os Command Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(LancamentoCommandHandler).Assembly,
    typeof(DeletarLancamentoCommandHandler).Assembly,
    typeof(AtualizarLancamentoCommandHandler).Assembly
));

// Configuração do gRPC através do SDK
builder.Services.AddGrpcSdk();

// **Adiciona os serviços gRPC (falta no código anterior)**
builder.Services.AddGrpc(); // Isso é necessário para que o gRPC funcione

// Swagger/OpenAPI para documentação
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia os controladores de API
app.MapControllers();

//// Mapeia o serviço gRPC (LauchService)
//app.MapGrpcService<LauchService>();

app.Run();

