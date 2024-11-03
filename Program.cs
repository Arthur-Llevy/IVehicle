using API.DTOs;
using API.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContxto>(options => 
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Mysql"))
    );
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => 
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    {
        return Results.Ok("Login com sucesso");
    }

    return Results.Unauthorized();
});

app.Run();

