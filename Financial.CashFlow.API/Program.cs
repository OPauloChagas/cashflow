using Financeiro.CashFlow.Business;
using Financeiro.CashFlow.Business.CommandHandlers;
using Financeiro.CashFlow.DataModels.Data;
using Financial.CashFlow.Sdk.Extensions;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
var builder = WebApplication.CreateBuilder(args);

// Adiciona os controladores (Web API)
builder.Services.AddControllers();


//// Configura��o de DbContext com InMemory
//builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
//    options.UseInMemoryDatabase("LancamentoDb"));


// Configurar o PostgreSQL
builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("Financeiro.CashFlow.DataModels") // Nome correto do projeto que cont�m o DbContext
    )
);

// Configura��o do MediatR para os Command Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(LancamentoCommandHandler).Assembly,
    typeof(DeletarLancamentoCommandHandler).Assembly,
    typeof(AtualizarLancamentoCommandHandler).Assembly
));

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ConnectionFactory()
    {
        HostName = configuration["RabbitMQ:HostName"],
        UserName = configuration["RabbitMQ:UserName"],
        Password = configuration["RabbitMQ:Password"]
    };
});

builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddSingleton<RabbitMQConsumer>();

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

//Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LancamentoAppDbContext>();
    dbContext.Database.Migrate(); 
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapeia os controladores de API
app.MapControllers();

//// Mapeia o servi�o gRPC (LauchService)
//app.MapGrpcService<LauchService>();

app.Run();

