using API.Dominio.Enteidades;
using API.DTOs;

namespace api.DTOs;

public interface IAdministradorInterface
{
    Administrador Incluir (AdministradorDTO administradorDTO);
    bool Login (LoginDTO loginDTO);
}