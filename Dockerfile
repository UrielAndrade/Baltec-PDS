# Estágio 1: Build da aplicação .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj e restaurar dependências
COPY ["Baltec.csproj", "./"]
RUN dotnet restore "Baltec.csproj"

# Copiar o restante dos arquivos e publicar
COPY . .
RUN dotnet publish "Baltec.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Imagem final de execução (Runtime enxuto)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Baltec.dll"]
