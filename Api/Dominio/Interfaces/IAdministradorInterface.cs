using API.Dominio.Enteidades;
using API.DTOs;

namespace api.DTOs;

public interface IAdministradorInterface
{
    Task<Administrador> Incluir (AdministradorDTO administradorDTO);
    Task<Administrador> Login (LoginDTO loginDTO);
    Task<List<Administrador>> Todos ();
    Task<Administrador> PegarPorId (int id);
    Task Excluir (int Id);
}