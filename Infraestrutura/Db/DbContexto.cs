using API.Dominio.Enteidades;
using Microsoft.EntityFrameworkCore;

namespace API.Infraestrutura.Contexto;

public class DbContxto: DbContext 
{
    private readonly IConfiguration _configuracaoAppSettings;

    public DbContxto(IConfiguration configuracaoAppSettings) 
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("Mysql")?.ToString();
            optionsBuilder.UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao));
        }
    }
}