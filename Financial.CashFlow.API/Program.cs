using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server.Services;
using Financial.CashFlow.Sdk.Extensions;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Adiciona os controladores (Web API)
builder.Services.AddControllers();


// Configura��o de DbContext com InMemory
builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
    options.UseInMemoryDatabase("LancamentoDb"));


// Configura��o do MediatR para os Command Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(LancamentoCommandHandler).Assembly,
    typeof(DeletarLancamentoCommandHandler).Assembly,
    typeof(AtualizarLancamentoCommandHandler).Assembly
));

// Configura��o do gRPC atrav�s do SDK
builder.Services.AddGrpcSdk();

// **Adiciona os servi�os gRPC (falta no c�digo anterior)**
builder.Services.AddGrpc(); // Isso � necess�rio para que o gRPC funcione

// Swagger/OpenAPI para documenta��o
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

//// Mapeia o servi�o gRPC (LauchService)
//app.MapGrpcService<LauchService>();

app.Run();

