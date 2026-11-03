using TarefasApi.Dados;
using TarefasApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var arquivo = builder.Configuration["Armazenamento:Arquivo"] ?? "dados/tarefas.json";
builder.Services.AddSingleton<ITarefaRepositorio>(_ => new RepositorioJson(arquivo));

// Libera o front-end (Next.js) para chamar a API pelo navegador.
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? ["http://localhost:3000"];
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();

app.MapGet("/", () => Results.Ok(new { api = "Tarefas", versao = "1.0", rotas = "/api/tarefas" }));
app.MapTarefas();

app.Run();

// Permite usar a classe Program em testes de integração.
public partial class Program;
