# CadastroClientes.Api

Microsservico responsavel pelo cadastro de clientes.

Escopo atual:
- cadastrar clientes;
- consultar cliente por id;
- validar dados de entrada;
- persistir em SQLite em memoria;
- publicar o evento `ClienteCadastrado`.

## Arquitetura

O projeto segue Clean Architecture com separacao por responsabilidade:

```text
src/
  CadastroClientes.Api
  CadastroClientes.Application
  CadastroClientes.Domain
  CadastroClientes.Infrastructure

tests/
  CadastroClientes.UnitTests
```

### Domain
- entidades;
- value objects;
- regras de negocio;
- excecoes de dominio.

### Application
- commands;
- queries;
- handlers;
- DTOs;
- contratos;
- eventos de integracao.

### Infrastructure
- Dapper;
- SQLite em memoria;
- RabbitMQ publisher;
- implementacoes de contratos;
- configuracoes externas.

### API
- controllers;
- middleware global de excecoes;
- Swagger/OpenAPI;
- health check;
- configuracao de DI.

## Tecnologias

- .NET 10
- C#
- ASP.NET Core
- MediatR
- CQRS
- Dapper
- SQLite em memoria
- RabbitMQ
- xUnit
- Swagger/OpenAPI
- Docker

## Execucao local

### Requisitos
- .NET SDK 10.0.400
- Docker, se quiser testar a imagem do container

### Rodar a API

```powershell
dotnet restore
dotnet build CadastroClientes.sln
dotnet run --project src/CadastroClientes.Api/CadastroClientes.Api.csproj
```

## Docker

Build da imagem:

```powershell
docker build -t cadastro-clientes-api:latest .
```

Execucao da imagem:

```powershell
docker run --rm -p 8080:8080 cadastro-clientes-api:latest
```

### Ambiente integrado

Para executar os tres microsservicos e o RabbitMQ, clone os tres repositorios como diretorios vizinhos:

```text
TesteParanaBanco/
  banco-cadastro-clientes-api/
  banco-proposta-credito/
  banco-cartao-credito/
```

A partir deste diretorio, execute:

```powershell
docker compose up --build
```

Portas do ambiente integrado:
- Cadastro: `http://localhost:5001`
- Proposta: `http://localhost:5002`
- Cartao: `http://localhost:5003`
- RabbitMQ Management: `http://localhost:15672` (`guest`/`guest`)

O RabbitMQ e acessado pelos microsservicos pelo nome do servico Docker `rabbitmq`.

## Endpoints

### POST /api/v1/clientes
Cria um novo cliente.

Entrada esperada:

```json
{
  "name": "Maria Silva",
  "cpf": "529.982.247-25",
  "email": "maria.silva@email.com",
  "score": 500
}
```

Resposta de sucesso:
- `201 Created`

Quando CPF ou e-mail ja estiver cadastrado:
- `409 Conflict`

### GET /api/v1/clientes/{id}
Consulta um cliente pelo id.

Resposta de sucesso:
- `200 OK`

Quando o cliente nao existe:
- `404 Not Found`

## Validacoes

As validacoes de entrada usam data annotations e regra de dominio:
- campos obrigatorios;
- CPF;
- e-mail;
- score entre 0 e 1000;
- dados invalidos.

## Banco de dados

- SQLite em memoria;
- acesso via Dapper;
- schema criado por SQL na inicializacao da API;
- a conexao precisa permanecer viva durante a execucao para manter o banco ativo.

## Mensageria

A API publica o evento `ClienteCadastrado` apos o cadastro com sucesso.

Contrato do evento:

```json
{
  "eventId": "guid",
  "occurredAt": "datetime",
  "clientId": "guid",
  "name": "string",
  "cpf": "string",
  "email": "string",
  "score": 500
}
```

Observacoes:
- nesta etapa o microsservico atua apenas como publisher.

## Testes

Projeto separado:
- `tests/CadastroClientes.UnitTests`

Executar:

```powershell
dotnet test CadastroClientes.sln
```

Cobertura de testes atual:
- dominio;
- application;
- controller;
- publicacao do evento via mock;
- conflito de unicidade do CPF no repositorio.

## Decisoes tecnicas

- Clean Architecture para separar responsabilidades e facilitar manutencao;
- MediatR para CQRS, deixando controller fino e separação de leituras e escritas.;
- Dapper para acesso leve ao SQLite;
- SQLite em memoria para evitar dependencia externa nesta fase;
- RabbitMQ abstraido na Application para nao acoplar o handler ao provider;
- Routing Key definida explicitamente em `Domain/Constants/Constants.cs`, dentro de `RabbitMqConstantes`, mantendo o publisher reutilizavel sem acoplar o contrato RabbitMQ ao nome da classe;
- testes unitarios com mocks, sem subir infraestrutura;
- middleware global para padronizar respostas de erro.

## Limitacoes atuais

- o banco e in-memory e vale apenas durante a execucao;

## Estrutura principal

### Endpoints
- `POST /api/v1/clientes`
- `GET /api/v1/clientes/{id}`
- `GET /health`

### Configuracoes
- `ConnectionStrings:CadastroClientes`
- `RabbitMq:HostName`
- `RabbitMq:Port`
- `RabbitMq:VirtualHost`
- `RabbitMq:UserName`
- `RabbitMq:Password`
- `RabbitMq:ExchangeName`
- `RabbitMq:ExchangeType`
