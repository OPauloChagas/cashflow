# Use a imagem base do ASP.NET Core runtime para .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use a imagem do SDK do .NET 8.0 para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências
COPY ["Financial.CashFlow.API/Financial.CashFlow.API.csproj", "Financial.CashFlow.API/"]
RUN dotnet restore "Financial.CashFlow.API/Financial.CashFlow.API.csproj"

# Copia o restante do código e compila
COPY . .
WORKDIR "/src/Financial.CashFlow.API"
RUN dotnet build "Financial.CashFlow.API.csproj" -c Release -o /app/build

# Publica a aplicação para a pasta de saída /app/publish
FROM build AS publish
RUN dotnet publish "Financial.CashFlow.API.csproj" -c Release -o /app/publish

# Usa a imagem base para rodar o app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Financial.CashFlow.API.dll"]
