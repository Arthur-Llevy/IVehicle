using API.Dominio.Enteidades;
using API.DTOs.ModelViews;

namespace API.Dominio.Intercaces;

public interface IVeiculoServico
{
    List<Veiculo>? Todos (int? pagina = 1, string? nome = null, string? marca = null);
    Veiculo? BuscaPorId (int id);
    void Incluir (Veiculo veiculo);
    void Atualizar (int Id, VeiculoDTO veiculo);
    void Excluir (int Id);
}