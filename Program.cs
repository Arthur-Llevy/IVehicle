using System.Security.Cryptography.X509Certificates;
using api.DTOs;
using API.Dominio.Enteidades;
using API.Dominio.Intercaces;
using API.Dominio.Servicos;
using API.DTOs;
using API.DTOs.ModelViews;
using API.Infraestrutura.Contexto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContxto>(options => 
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Mysql"))
    );
});

builder.Services.AddScoped<IVeiculoInterface, VeiculoServico>();
builder.Services.AddScoped<IAdministradorInterface, AdministradorServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home()));

ErrosDeValidacao Validar(VeiculoDTO veiculo)
{
    var validacao = new ErrosDeValidacao

    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculo.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio.");

    if (string.IsNullOrEmpty(veiculo.Marca))
        validacao.Mensagens.Add("A marca não pode ser vazio.");

    if (veiculo.Ano <= 1950)
        validacao.Mensagens.Add("Por favor, insira um ano válido.");

   return validacao;
}

app.MapGet("/administradores", (IAdministradorInterface administradorServico) => 
{   
    var adms = administradorServico.Todos();
    List<AdministradorModelView> results = new List<AdministradorModelView>();

    foreach (Administrador adm in adms)
    {
        results.Add(new AdministradorModelView
        {
            Email = adm.Email,
            Id = adm.Id
        });
    }
    return Results.Ok(results);
}).WithTags("Administrador");

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorInterface adiministradorServico) => 
{   
    bool validacao = adiministradorServico.Login(loginDTO);

    if (validacao)
        return Results.Ok();

    return Results.Unauthorized();
}).WithTags("Login");

app.MapGet("/administradores/{id}", (int id, IAdministradorInterface administradorServico) => 
{
    var administrador = administradorServico.PegarPorId(id);
    AdministradorModelView adm = new AdministradorModelView 
    {
        Email = administrador.Email,
        Id = administrador.Id
    };

    return Results.Ok(adm);
}).WithTags("Administrador");

app.MapDelete("/administradores/{id}", (int id, IAdministradorInterface administradorServico) => 
{
    administradorServico.Excluir(id);
    return Results.Ok();
}).WithTags("Administrador");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorInterface adiministradorServico) => 
{
    var novoAdministrador = adiministradorServico.Incluir(administradorDTO);

    return Results.Ok(novoAdministrador);
}).WithTags("Administrador");

app.MapPost("/veiculo", ([FromBody] VeiculoDTO veiculo, IVeiculoInterface veiculoServico) => 
{
    var validacao = Validar(veiculo);  

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    Veiculo novoVeiculo = new Veiculo 
    {
        Nome = veiculo.Nome,
        Ano = veiculo.Ano,
        Marca = veiculo.Marca
    };
    veiculoServico.Incluir(novoVeiculo);

    return Results.Created($"/veiculo/{novoVeiculo.Id}", veiculo);
}).WithTags("Veiculo");

app.MapGet("/veiculo/", ([FromQuery] int? pagina, IVeiculoInterface veiculoServico) => 
{   
    if (pagina != null)
    {
        return Results.Ok(veiculoServico.Todos((int) pagina));
    }

    return Results.Ok(veiculoServico.Todos(0));
}).WithTags("Veiculo");

app.MapGet("/veiculo/{id}", ([FromQuery] int id, IVeiculoInterface veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPatch("/veiculo/{id}", ([FromQuery] int Id, [FromBody] VeiculoDTO veiculo, IVeiculoInterface veiculoServico) => 
{
    var validacao = Validar(veiculo);  

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);
    
    veiculoServico.Atualizar(Id, veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");

app.MapDelete("/veiculo", ([FromQuery] int Id, IVeiculoInterface veiculoServico) => 
{   
    veiculoServico.Excluir(Id);

    return Results.NoContent();
}).WithTags("Veiculo");

app.UseSwagger();
app.UseSwaggerUI();
app.Run();

