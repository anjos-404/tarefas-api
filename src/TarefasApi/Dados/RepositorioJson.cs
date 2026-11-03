using System.Text.Json;
using TarefasApi.Models;

namespace TarefasApi.Dados;

/// <summary>
/// Guarda as tarefas num arquivo JSON. Simples, sem dependências, e seguro
/// para várias requisições ao mesmo tempo (um lock protege leitura e escrita).
/// </summary>
public class RepositorioJson : ITarefaRepositorio
{
    private static readonly JsonSerializerOptions Opcoes = new() { WriteIndented = true };

    private readonly string _arquivo;
    private readonly SemaphoreSlim _trava = new(1, 1);
    private readonly TimeProvider _relogio;
    private List<Tarefa>? _tarefas;

    public RepositorioJson(string arquivo, TimeProvider? relogio = null)
    {
        _arquivo = arquivo;
        _relogio = relogio ?? TimeProvider.System;
    }

    public async Task<IReadOnlyList<Tarefa>> ListarAsync(bool? concluida = null)
    {
        return await ComTravaAsync(tarefas => tarefas
            .Where(t => concluida is null || t.Concluida == concluida)
            .OrderByDescending(t => t.CriadaEm)
            .ThenByDescending(t => t.Id)
            .Select(Copiar)
            .ToList());
    }

    public async Task<Tarefa?> ObterAsync(int id)
    {
        return await ComTravaAsync(tarefas =>
        {
            var tarefa = tarefas.FirstOrDefault(t => t.Id == id);
            return tarefa is null ? null : Copiar(tarefa);
        });
    }

    public async Task<Tarefa> CriarAsync(string titulo, string? descricao)
    {
        return await ComTravaAsync(async tarefas =>
        {
            var tarefa = new Tarefa
            {
                Id = tarefas.Count == 0 ? 1 : tarefas.Max(t => t.Id) + 1,
                Titulo = titulo,
                Descricao = descricao,
                CriadaEm = _relogio.GetUtcNow().UtcDateTime,
            };
            tarefas.Add(tarefa);
            await SalvarAsync(tarefas);
            return Copiar(tarefa);
        });
    }

    public async Task<Tarefa?> AtualizarAsync(int id, string titulo, string? descricao, bool concluida)
    {
        return await ComTravaAsync(async tarefas =>
        {
            var tarefa = tarefas.FirstOrDefault(t => t.Id == id);
            if (tarefa is null) return null;

            if (concluida && !tarefa.Concluida)
                tarefa.ConcluidaEm = _relogio.GetUtcNow().UtcDateTime;
            else if (!concluida)
                tarefa.ConcluidaEm = null;

            tarefa.Titulo = titulo;
            tarefa.Descricao = descricao;
            tarefa.Concluida = concluida;
            await SalvarAsync(tarefas);
            return Copiar(tarefa);
        });
    }

    public async Task<bool> RemoverAsync(int id)
    {
        return await ComTravaAsync(async tarefas =>
        {
            var removidas = tarefas.RemoveAll(t => t.Id == id);
            if (removidas > 0) await SalvarAsync(tarefas);
            return removidas > 0;
        });
    }

    private async Task<T> ComTravaAsync<T>(Func<List<Tarefa>, T> acao) =>
        await ComTravaAsync(tarefas => Task.FromResult(acao(tarefas)));

    private async Task<T> ComTravaAsync<T>(Func<List<Tarefa>, Task<T>> acao)
    {
        await _trava.WaitAsync();
        try
        {
            _tarefas ??= await CarregarAsync();
            return await acao(_tarefas);
        }
        finally
        {
            _trava.Release();
        }
    }

    private async Task<List<Tarefa>> CarregarAsync()
    {
        if (!File.Exists(_arquivo)) return new List<Tarefa>();
        await using var leitura = File.OpenRead(_arquivo);
        return await JsonSerializer.DeserializeAsync<List<Tarefa>>(leitura, Opcoes) ?? new List<Tarefa>();
    }

    private async Task SalvarAsync(List<Tarefa> tarefas)
    {
        var pasta = Path.GetDirectoryName(Path.GetFullPath(_arquivo));
        if (!string.IsNullOrEmpty(pasta)) Directory.CreateDirectory(pasta);

        // Escreve num arquivo temporário e troca no fim: se o processo cair no meio,
        // o arquivo antigo continua inteiro.
        var temporario = _arquivo + ".tmp";
        await using (var escrita = File.Create(temporario))
        {
            await JsonSerializer.SerializeAsync(escrita, tarefas, Opcoes);
        }
        File.Move(temporario, _arquivo, overwrite: true);
    }

    private static Tarefa Copiar(Tarefa t) => new()
    {
        Id = t.Id,
        Titulo = t.Titulo,
        Descricao = t.Descricao,
        Concluida = t.Concluida,
        CriadaEm = t.CriadaEm,
        ConcluidaEm = t.ConcluidaEm,
    };
}
