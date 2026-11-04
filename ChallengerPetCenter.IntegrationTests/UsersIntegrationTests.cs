using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.IntegrationTests.Fixtures;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection("Integration collection")]
    public class UsersIntegrationTests
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public UsersIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_QuandoExistemUsuarios_RetornaOkPaginadoComLinks()
        {
            // Arrange
            await CreateUserAsync($"list_{Guid.NewGuid():N}@email.com");

            // Act
            var response = await _client.GetAsync("/api/users?page=1&pageSize=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("items", body, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("_links", body, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("total", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetUserById_QuandoNaoExiste_RetornaNotFoundProblemDetails()
        {
            // Arrange / Act
            var response = await _client.GetAsync("/api/users/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("title", body, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("detail", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateUser_QuandoEmailDuplicado_RetornaBadRequest()
        {
            // Arrange
            var email = $"dup_{Guid.NewGuid():N}@email.com";
            await CreateUserAsync(email);

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", new CreateUserDto
            {
                Nome = "Outro",
                Email = email,
                Senha = "abc",
                TipoUsuario = "TUTOR"
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_QuandoDadosValidos_RetornaCreated()
        {
            // Arrange / Act
            var response = await CreateUserAsync($"ok_{Guid.NewGuid():N}@email.com");

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
            Assert.True(payload.TryGetProperty("data", out var data));
            Assert.True(data.GetProperty("id").GetInt32() > 0);
            Assert.True(payload.TryGetProperty("_links", out _));
        }

        private async Task<HttpResponseMessage> CreateUserAsync(string email)
        {
            var response = await _client.PostAsJsonAsync("/api/users", new CreateUserDto
            {
                Nome = "Integracao",
                Email = email,
                Senha = "123456",
                TipoUsuario = "TUTOR"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return response;
        }
    }
}
