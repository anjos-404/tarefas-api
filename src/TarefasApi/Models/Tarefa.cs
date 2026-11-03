namespace TarefasApi.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Concluida { get; set; }
    public DateTime CriadaEm { get; set; }
    public DateTime? ConcluidaEm { get; set; }
}

public record CriarTarefaRequest(string? Titulo, string? Descricao);

public record AtualizarTarefaRequest(string? Titulo, string? Descricao, bool Concluida);

public record ErroResponse(string Erro);
