using API.Dominio.Enteidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        var adm = new Administrador();

        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";

        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
    }
}