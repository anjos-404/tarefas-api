namespace TarefasApi.Models;

public static class Validacao
{
    public const int TamanhoMaximoTitulo = 120;
    public const int TamanhoMaximoDescricao = 500;

    /// <summary>Devolve a mensagem de erro, ou null se os dados forem válidos.</summary>
    public static string? Validar(string? titulo, string? descricao)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return "O título é obrigatório.";
        if (titulo.Trim().Length > TamanhoMaximoTitulo)
            return $"O título pode ter no máximo {TamanhoMaximoTitulo} caracteres.";
        if (descricao is not null && descricao.Trim().Length > TamanhoMaximoDescricao)
            return $"A descrição pode ter no máximo {TamanhoMaximoDescricao} caracteres.";
        return null;
    }

    public static string? LimparDescricao(string? descricao) =>
        string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
}
