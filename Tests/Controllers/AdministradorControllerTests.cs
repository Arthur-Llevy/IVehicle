using System.Net;
using System.Text;
using API.DTOs;
using API.DTOs.ModelViews;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

public class AdministradorControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient _httpClient;
    private WebApplicationFactory<Program> _factory;

    public AdministradorControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task When_Get_In_Administradores_Should_Return_A_List_Of_AdministradoresModelView()
    {
        var result = await _httpClient.GetAsync("/administradores");
        var resultJson = await result.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsAssignableFrom<List<AdministradorModelView>>(JsonConvert.DeserializeObject<List<AdministradorModelView>>(resultJson));
    }

    [Fact]
    public async Task When_Post_In_Administradores_Should_Return_A_New_Administrador()
    {
        AdministradorDTO newAdministrador = new AdministradorDTO 
        {
            Email = "email@email.com",
            Perfil = "perfil",
            Senha = "123"
        };

        StringContent data = new StringContent(JsonConvert.SerializeObject(newAdministrador), Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync("/administradores", data);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var resultDataJson = JsonConvert.DeserializeObject<AdministradorModelView>(await result.Content.ReadAsStringAsync());

        Assert.IsAssignableFrom<AdministradorModelView>(resultDataJson);
    }
}