# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia tudo da pasta do serviço para dentro do container
COPY . .

# Restaura pacotes e publica em modo Release
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expondo a porta padrão
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Ajuste o nome do dll gerado pelo seu projeto
ENTRYPOINT ["dotnet", "Venda-Servico.dll"]
