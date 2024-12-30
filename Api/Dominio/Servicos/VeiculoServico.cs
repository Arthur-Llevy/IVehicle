using API.Dominio.Enteidades;
using API.Dominio.Intercaces;
using API.DTOs;
using API.Infraestrutura.Contexto;

namespace API.Dominio.Servicos;

public class VeiculoServico : IVeiculoInterface
{
    private readonly DbContxto _contexto;
    public VeiculoServico (DbContxto contexto) 
    {
        _contexto = contexto;
    }
    public void Atualizar(int Id, VeiculoDTO veiculo)
    {
        var veiculoAtualizar = BuscaPorId(Id);

        if (veiculoAtualizar != null) 
        {
            veiculoAtualizar.Ano = veiculo.Ano;
            veiculoAtualizar.Marca = veiculo.Marca;
            veiculoAtualizar.Nome = veiculo.Nome;

            _contexto.Veiculos.Update(veiculoAtualizar);
            _contexto.SaveChanges();
        };
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.Where(x => x.Id == id).FirstOrDefault();
    }

    public void Excluir(int Id)
    {
        var veiculo = BuscaPorId(Id);

        if (veiculo != null)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public List<Veiculo>? Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos;
        int itensPorPargina = 10;

        if (pagina != null) 
        {
            query.Skip(((int) pagina - 1) * itensPorPargina).Take(itensPorPargina);
        }

        if (!string.IsNullOrEmpty(nome))
        {
            return query.Where(x => x.Nome.ToLower() == nome).ToList();
        }

        return query.ToList();
    }
}