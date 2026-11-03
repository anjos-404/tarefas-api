# Tarefas — API

API REST de tarefas (to-do) em **C# com ASP.NET Core 8** (minimal APIs). É o back-end do [tarefas-web](https://github.com/anjos-404/tarefas-web), o front em Next.js.

## Rotas

| Método | Rota | O que faz |
|---|---|---|
| GET | `/api/tarefas` | lista as tarefas (mais novas primeiro); aceita `?concluida=true` ou `false` |
| GET | `/api/tarefas/{id}` | uma tarefa |
| POST | `/api/tarefas` | cria: `{ "titulo": "...", "descricao": "..." }` |
| PUT | `/api/tarefas/{id}` | atualiza título, descrição e se está concluída |
| DELETE | `/api/tarefas/{id}` | remove |

Respostas de erro vêm no formato `{ "erro": "mensagem" }`, com 400 (dados inválidos) ou 404 (tarefa não existe).

## Como rodar

Precisa do [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd src/TarefasApi
dotnet run
```

A API sobe em `http://localhost:5000`. O arquivo `TarefasApi.http` tem exemplos prontos de cada chamada.

## Decisões do projeto

- **Repositório por interface** (`ITarefaRepositorio`): os endpoints não sabem onde os dados ficam. Hoje é um arquivo JSON (`RepositorioJson`), e trocar por SQLite ou SQL Server com Entity Framework é só uma nova classe registrada no `Program.cs`.
- **Escrita segura**: grava num arquivo temporário e troca no fim, para não corromper os dados se o processo cair. Um lock evita conflito entre requisições simultâneas.
- **Validação** centralizada (título obrigatório até 120 caracteres, descrição até 500).
- **CORS** liberado só para o front (`http://localhost:3000`), configurável em `appsettings.json`.

## Estrutura

```
src/TarefasApi/Program.cs                    configuração e injeção de dependência
src/TarefasApi/Endpoints/TarefasEndpoints.cs rotas da API
src/TarefasApi/Dados/ITarefaRepositorio.cs   contrato do armazenamento
src/TarefasApi/Dados/RepositorioJson.cs      armazenamento em arquivo JSON
src/TarefasApi/Models/                       entidade, requisições e validação
```

## Próximos passos

- Trocar o arquivo JSON por SQLite com Entity Framework Core
- Testes de integração com xUnit e `WebApplicationFactory`
- Documentação com Swagger

## Tecnologias

C# · .NET 8 · ASP.NET Core Minimal APIs · System.Text.Json
