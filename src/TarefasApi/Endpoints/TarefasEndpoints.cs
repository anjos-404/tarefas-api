using TarefasApi.Dados;
using TarefasApi.Models;

namespace TarefasApi.Endpoints;

public static class TarefasEndpoints
{
    public static RouteGroupBuilder MapTarefas(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/tarefas");

        // GET /api/tarefas?concluida=true|false
        grupo.MapGet("/", async (bool? concluida, ITarefaRepositorio repo) =>
            Results.Ok(await repo.ListarAsync(concluida)));

        grupo.MapGet("/{id:int}", async (int id, ITarefaRepositorio repo) =>
            await repo.ObterAsync(id) is { } tarefa
                ? Results.Ok(tarefa)
                : NaoEncontrada(id));

        grupo.MapPost("/", async (CriarTarefaRequest dados, ITarefaRepositorio repo) =>
        {
            if (Validacao.Validar(dados.Titulo, dados.Descricao) is { } erro)
                return Results.BadRequest(new ErroResponse(erro));

            var tarefa = await repo.CriarAsync(dados.Titulo!.Trim(), Validacao.LimparDescricao(dados.Descricao));
            return Results.Created($"/api/tarefas/{tarefa.Id}", tarefa);
        });

        grupo.MapPut("/{id:int}", async (int id, AtualizarTarefaRequest dados, ITarefaRepositorio repo) =>
        {
            if (Validacao.Validar(dados.Titulo, dados.Descricao) is { } erro)
                return Results.BadRequest(new ErroResponse(erro));

            var tarefa = await repo.AtualizarAsync(
                id, dados.Titulo!.Trim(), Validacao.LimparDescricao(dados.Descricao), dados.Concluida);
            return tarefa is null ? NaoEncontrada(id) : Results.Ok(tarefa);
        });

        grupo.MapDelete("/{id:int}", async (int id, ITarefaRepositorio repo) =>
            await repo.RemoverAsync(id) ? Results.NoContent() : NaoEncontrada(id));

        return grupo;
    }

    private static IResult NaoEncontrada(int id) =>
        Results.NotFound(new ErroResponse($"Tarefa {id} não encontrada."));
}
