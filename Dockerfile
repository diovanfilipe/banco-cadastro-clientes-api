# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY CadastroClientes.sln ./
COPY src/CadastroClientes.Domain/CadastroClientes.Domain.csproj src/CadastroClientes.Domain/
COPY src/CadastroClientes.Application/CadastroClientes.Application.csproj src/CadastroClientes.Application/
COPY src/CadastroClientes.Infrastructure/CadastroClientes.Infrastructure.csproj src/CadastroClientes.Infrastructure/
COPY src/CadastroClientes.Api/CadastroClientes.Api.csproj src/CadastroClientes.Api/

RUN dotnet restore src/CadastroClientes.Api/CadastroClientes.Api.csproj

COPY . .
RUN dotnet publish src/CadastroClientes.Api/CadastroClientes.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CadastroClientes.Api.dll"]
