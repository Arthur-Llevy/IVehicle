using System.ComponentModel;
using api.DTOs;
using API.Dominio.Enteidades;
using API.DTOs;
using API.Infraestrutura.Contexto;

namespace API.Dominio.Servicos;

public class AdministradorServico : IAdministradorInterface
{
    private readonly DbContxto _contexto;

    public AdministradorServico(DbContxto contexto)
    {
        _contexto = contexto;
    }
    public List<Administrador> Todos ()
    {
        return _contexto.Administradores.ToList(); 
    }

    public Administrador PegarPorId (int id)
    {
        var administrador = _contexto.Administradores.Where(x => x.Id == id).FirstOrDefault();

        return administrador;
    }

    public void Excluir (int id)
    {
        var administradorParaEcluir = _contexto.Administradores.Where(x => x.Id == id).FirstOrDefault();

        if (administradorParaEcluir != null)
        {
            _contexto.Administradores.Remove(administradorParaEcluir);
            _contexto.SaveChanges();
        }

    }
    public Administrador Incluir(AdministradorDTO administradorDTO)
    {
        var novoAdministrador = new Administrador();
        novoAdministrador.Email = administradorDTO.Email;
        novoAdministrador.Perfil = administradorDTO.Perfil;
        novoAdministrador.Senha = administradorDTO.Senha;
        
        _contexto.Administradores.Add(novoAdministrador);
        _contexto.SaveChanges();

        return novoAdministrador;
    }

    public bool Login (LoginDTO loginDTO)
    {
        var administrador = _contexto.Administradores.Where(x => x.Email == loginDTO.Email && x.Senha == loginDTO.Senha);

        if (administrador != null) 
        {
            return true;
        }

        return false;
    }
}