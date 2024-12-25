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

builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/login", (LoginDTO loginDTO) => 
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    {
        return Results.Ok("Login com sucesso");
    }

    return Results.Unauthorized();
}).WithTags("Login");

app.MapPost("/veiculo", ([FromBody] VeiculoDTO veiculo, IVeiculoServico veiculoServico) => 
{
    Veiculo novoVeiculo = new Veiculo 
    {
        Nome = veiculo.Nome,
        Ano = veiculo.Ano,
        Marca = veiculo.Marca
    };
    veiculoServico.Incluir(novoVeiculo);

    return Results.Created($"/veiculo/{novoVeiculo.Id}", veiculo);
}).WithTags("Veiculo");

app.MapGet("/veiculo/", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => 
{   
    if (pagina != null)
    {
        return Results.Ok(veiculoServico.Todos((int) pagina));
    }

    return Results.Ok(veiculoServico.Todos(0));
}).WithTags("Veiculo");

app.MapGet("/veiculo/{id}", ([FromQuery] int id, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPatch("/veiculo/{id}", ([FromQuery] int Id, [FromBody] VeiculoDTO veiculo, IVeiculoServico veiculoServico) => 
{
    veiculoServico.Atualizar(Id, veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");

app.MapDelete("/veiculo", ([FromQuery] int Id, IVeiculoServico veiculoServico) => 
{   
    veiculoServico.Excluir(Id);

    return Results.NoContent();
}).WithTags("Veiculo");

app.UseSwagger();
app.UseSwaggerUI();
app.Run();

