using System.Net;
using System.Text;
using API.Dominio.Enteidades;
using API.DTOs;
using API.DTOs.ModelViews;
using AutoBogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

public class AdministradorControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private HttpClient _httpClient;
    private WebApplicationFactory<Program> _factory;
    private string _token = "";

    public AdministradorControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

public async Task InitializeAsync()
{
    var administradorFaker = new AutoFaker<AdministradorDTO>();
    
    StringContent administradorData = new StringContent(JsonConvert.SerializeObject(administradorFaker.Generate()), Encoding.UTF8, "application/json");
    var resultCreateAdministrador = await _httpClient.PostAsync("/administradores", administradorData);

    if (resultCreateAdministrador.IsSuccessStatusCode)
    {
        var administradorDataJson = JsonConvert.DeserializeObject<Administrador>(await resultCreateAdministrador.Content.ReadAsStringAsync());

        if (administradorDataJson != null)
        {
            var login = new LoginDTO 
            {
                Email = administradorDataJson.Email,
                Senha = administradorDataJson.Senha
            };

            StringContent loginStringContent = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync("/administradores/login", loginStringContent);
            if (result.IsSuccessStatusCode)
            {
                var jsonResponse = await result.Content.ReadAsStringAsync();
                Console.WriteLine("Login Response: " + jsonResponse); // Para depuração

                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                if (data != null && data.TryGetValue("token", out var token))
                {
                    _token = token;
                }
                else
                {
                    Console.WriteLine("Token não encontrado na resposta.");
                }
            }
            else
            {
                Console.WriteLine($"Erro ao fazer login: {result.StatusCode}");
            }
        }
        else
        {
            Console.WriteLine("Erro ao desserializar dados do administrador.");
        }
    }
    else
    {
        Console.WriteLine($"Erro ao criar administrador: {resultCreateAdministrador.StatusCode}");
    }
}

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task When_Get_In_Administradores_Should_Return_A_List_Of_AdministradoresModelView()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        var result = await _httpClient.GetAsync("/administradores");

        if (result.IsSuccessStatusCode)
        {
            var resultJson = await result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.IsAssignableFrom<List<AdministradorModelView>>(JsonConvert.DeserializeObject<List<AdministradorModelView>>(resultJson));
        }
    }

    [Fact]
    public async Task When_Post_In_Administradores_Should_Return_A_New_Administrador_If_Already_Not_Exists()
    {
        var faker = new AutoFaker<AdministradorDTO>();
        var newAdministrador = faker.Generate();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        StringContent data = new StringContent(JsonConvert.SerializeObject(newAdministrador), Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync("/administradores", data);

        if (result.IsSuccessStatusCode)
        {
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var resultDataJson = JsonConvert.DeserializeObject<AdministradorModelView>(await result.Content.ReadAsStringAsync());
            Assert.IsAssignableFrom<AdministradorModelView>(resultDataJson);
        }
    }

    [Fact]
    public async Task When_Post_In_Administradores_Should_Return_A_New_Invalid_Operation_If_The_Administrator_Already_Exists ()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        AdministradorDTO newAdministrador = new AdministradorDTO 
        {
            Email = "email@email.com",
            Perfil = "perfil",
            Senha = "123"
        };
        StringContent data = new StringContent(JsonConvert.SerializeObject(newAdministrador), Encoding.UTF8, "application/json");

        await _httpClient.PostAsync("/administradores", data);
        
        var result = await _httpClient.PostAsync("/administradores", data);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task When_Get_With_Id_Param_In_Administradores_Should_Return_One_Administrador_If_Exists()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        var result = await _httpClient.GetAsync("/administradores/4");

        if (result.IsSuccessStatusCode)
        {
            var jsonResult = JsonConvert.DeserializeObject<AdministradorModelView>(await result.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.IsAssignableFrom<AdministradorModelView>(jsonResult);
        }
    }

    [Fact]
    public async Task When_Get_With_Id_Param_In_Administradores_Should_Return_Not_Found_Status_If_Not_Exists()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        var result = await _httpClient.GetAsync("/administradores/99999999");

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task When_Delete_With_Id_Param_In_Administradores_Should_Return_NotContent_If_The_Administrador_Exists()
    {
        var faker = new AutoFaker<AdministradorDTO>();
        var newAdministrador = faker.Generate();

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        StringContent data = new StringContent(JsonConvert.SerializeObject(newAdministrador), Encoding.UTF8, "application/json");

        var postResult = await _httpClient.PostAsync("/administradores", data);

        if (postResult.IsSuccessStatusCode)
        {
            var resultJson = await postResult.Content.ReadAsStringAsync();
            var resultDataJson = JsonConvert.DeserializeObject<AdministradorModelView>(resultJson);

            if (resultDataJson != null)
            {
                var deleteResult = await _httpClient.DeleteAsync($"/administradores/{resultDataJson.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResult.StatusCode);
            }
        }
    }

    [Fact]
    public async Task When_Delete_With_Id_Param_In_Administraodres_Sould_Return_Bad_Request_If_Not_Exists()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

        var deleteResult = await _httpClient.DeleteAsync($"/administradores/999");

        if (deleteResult.IsSuccessStatusCode)
        {
            Assert.Equal(HttpStatusCode.BadRequest, deleteResult.StatusCode);
        }
    }
}