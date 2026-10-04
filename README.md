# TaskFlow

API de gerenciamento de projetos, tarefas e usuários construída com **.NET 10**, seguindo uma arquitetura em camadas (inspirada em Clean Architecture).

![CI - Build e Testes](https://github.com/jvitorsi-dev/TaskFlow/actions/workflows/ci.yml/badge.svg)

## Estrutura do projeto

| Projeto | Responsabilidade |
| --- | --- |
| `TaskFlow.Domain` | Entidades (`Projeto`, `Tarefa`, `Usuario`), enums, exceções e contratos de repositório. Sem dependências externas. |
| `TaskFlow.Application` | Casos de uso (CQRS simplificado: um caso de uso por operação), commands, DTOs e serviços de orquestração. |
| `TaskFlow.Infrastructure` | Persistência com Entity Framework Core (SQL Server), configurações de mapeamento, migrations e repositórios. |
| `TaskFlow.Api` | API REST (controllers), tratamento global de exceções (`ProblemDetails`) e Swagger. |
| `TaskFlow.Tests` | Testes automatizados (xUnit + NSubstitute + EF Core In-Memory). |

## Como executar a API

```bash
dotnet run --project TaskFlow.Api
```

> É necessário um SQL Server acessível via connection string `DefaultConnection` no `appsettings.json`.

## Como executar os testes

```bash
# Todos os testes
dotnet test TaskFlow.Tests/TaskFlow.Tests.csproj

# Com detalhes por teste
dotnet test TaskFlow.Tests/TaskFlow.Tests.csproj --logger "console;verbosity=detailed"
```

A suíte cobre:

- **Domínio** – regras de validação e a máquina de estados de `Projeto`, `Tarefa` e `Usuario` (Planejado → Em Andamento → Concluído / Pendente → Em Andamento → Concluída).
- **Aplicação** – todos os casos de uso com repositórios e UnitOfWork dublados via NSubstitute.
- **API** – controllers (mapeamento de resultados HTTP) e o `GlobalExceptionHandler` (400/404/500 como `ProblemDetails`).
- **Infraestrutura** – `TaskFlowDbContext` e repositórios contra um banco EF Core In-Memory.

Testes marcados com `Skip` em `TaskFlow.Tests/KnownIssuesTests.cs` documentam bugs conhecidos no código atual; ao corrigir um deles, remova o `Skip` para que o teste passe a proteger o comportamento correto.

## Integração contínua

O workflow [`.github/workflows/ci.yml`](.github/workflows/ci.yml) executa restore, build e testes a cada push/PR e publica os resultados (TRX) como artefato.
