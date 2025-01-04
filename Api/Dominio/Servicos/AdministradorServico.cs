using api.DTOs;
using API.Dominio.Enteidades;
using API.DTOs;
using API.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace API.Dominio.Servicos;

public class AdministradorServico : IAdministradorInterface
{
    private readonly DbContxto _contexto;

    public AdministradorServico(DbContxto contexto)
    {
        _contexto = contexto;
    }
    public async Task<List<Administrador>> Todos ()
    {
        return await _contexto.Administradores.ToListAsync(); 
    }

    public async Task<Administrador> PegarPorId (int id)
    {
        var administrador = await _contexto.Administradores.Where(x => x.Id == id).FirstOrDefaultAsync();

        return administrador;
    }

    public async Task Excluir (int id)
    {
        var administradorParaEcluir = await _contexto.Administradores.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (administradorParaEcluir != null)
        {
            _contexto.Administradores.Remove(administradorParaEcluir);
            await _contexto.SaveChangesAsync();
        }
        throw new InvalidOperationException("Não existe este administrador no sistema.");
    }
    public async Task<Administrador> Incluir(AdministradorDTO administradorDTO)
    {
        var busca = await _contexto.Administradores.Where(x => x.Email == administradorDTO.Email).FirstOrDefaultAsync();

        if (busca == null)
        {
            var novoAdministrador = new Administrador();
            novoAdministrador.Email = administradorDTO.Email;
            novoAdministrador.Perfil = administradorDTO.Perfil;
            novoAdministrador.Senha = administradorDTO.Senha;
            
            _contexto.Administradores.Add(novoAdministrador);
            await _contexto.SaveChangesAsync();

            return novoAdministrador;
        } 
        throw new InvalidOperationException("Um administrador com esse e-mail já está cadastrado");
    }

    public async Task<Administrador> Login (LoginDTO loginDTO)
    {
        var administrador = await _contexto.Administradores.Where(x => x.Email == loginDTO.Email && x.Senha == loginDTO.Senha).FirstOrDefaultAsync();

        return administrador;
    }
}