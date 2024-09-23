using Financeiro.CashFlow.Server.Services;
using Financeiro.CashFlow.DataModels.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração de DbContext com InMemory
//builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
//    options.UseInMemoryDatabase("LancamentoDb"));

// Configura o PostgreSQL no Entity Framework
builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("Financeiro.CashFlow.DataModels") // Nome correto do projeto que contém o DbContext
    )
);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<LauchService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
