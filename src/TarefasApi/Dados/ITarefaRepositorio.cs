using TarefasApi.Models;

namespace TarefasApi.Dados;

/// <summary>
/// Onde as tarefas ficam guardadas. Os endpoints só conhecem esta interface,
/// então trocar o armazenamento (arquivo, SQLite, SQL Server) não muda a API.
/// </summary>
public interface ITarefaRepositorio
{
    Task<IReadOnlyList<Tarefa>> ListarAsync(bool? concluida = null);
    Task<Tarefa?> ObterAsync(int id);
    Task<Tarefa> CriarAsync(string titulo, string? descricao);
    Task<Tarefa?> AtualizarAsync(int id, string titulo, string? descricao, bool concluida);
    Task<bool> RemoverAsync(int id);
}
