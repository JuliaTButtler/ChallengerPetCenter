using System.Net;
using System.Net.Http.Json;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.IntegrationTests.Fixtures;
using ChallengerPetCenter.Middleware;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection(ApiCollection.Name)]
    public class UsersEndpointTests
    {
        private readonly HttpClient _client;

        public UsersEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add(ApiKeyMiddleware.HeaderName, CustomWebApplicationFactory.TestApiKey);
        }

        [Fact]
        public async Task GetUsers_QuandoAutenticado_DeveRetornarOk()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_QuandoDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Nome = "Integração User",
                Email = $"integracao-{Guid.NewGuid():N}@email.com",
                Senha = "senha123",
                TipoUsuario = "TUTOR"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Email, created!.Email);
        }

        [Fact]
        public async Task CreateUser_QuandoEmailInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Nome = "Sem Email",
                Email = "invalido",
                Senha = "senha",
                TipoUsuario = "TUTOR"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_QuandoNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var idInexistente = 999999;

            // Act
            var response = await _client.GetAsync($"/api/users/{idInexistente}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUsers_QuandoSemApiKey_DeveRetornarUnauthorized()
        {
            // Arrange
            var clientSemAuth = new CustomWebApplicationFactory().CreateClient();

            // Act
            var response = await clientSemAuth.GetAsync("/api/users");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
