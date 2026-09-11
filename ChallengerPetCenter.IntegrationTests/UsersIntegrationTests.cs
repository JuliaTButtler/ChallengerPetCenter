using System.Net;
using System.Net.Http.Json;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.IntegrationTests.Fixtures;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection("Integration collection")]
    public class UsersIntegrationTests
    {
        private readonly HttpClient _client;

        public UsersIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_QuandoExistemUsuarios_RetornaOk()
        {
            // Arrange
            await CreateUserAsync($"list_{Guid.NewGuid():N}@email.com");

            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
            Assert.NotNull(users);
            Assert.NotEmpty(users!);
        }

        [Fact]
        public async Task GetUserById_QuandoNaoExiste_RetornaNotFound()
        {
            // Arrange / Act
            var response = await _client.GetAsync("/api/users/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
            var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            Assert.NotNull(user);
            Assert.True(user!.Id > 0);
        }

        [Fact]
        public async Task HealthLive_QuandoApiSobe_RetornaOk()
        {
            // Arrange / Act
            var response = await _client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
