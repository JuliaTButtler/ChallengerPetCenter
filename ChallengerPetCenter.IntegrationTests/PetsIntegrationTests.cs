using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChallengerPetCenter.DTOs.Auth;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.IntegrationTests.Fixtures;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection("Integration collection")]
    public class PetsIntegrationTests
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PetsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreatePet_QuandoSemToken_RetornaUnauthorized()
        {
            // Arrange / Act
            var response = await _client.PostAsJsonAsync("/api/pets", new
            {
                nome = "Mel",
                especie = "Cachorro",
                userId = 1
            });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreatePet_QuandoComToken_RetornaCreatedComHateoas()
        {
            // Arrange
            var email = $"pet_{Guid.NewGuid():N}@email.com";
            var userId = await CreateUserAndGetIdAsync(email, "senha123");
            var token = await LoginAndGetTokenAsync(email, "senha123");

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/pets")
            {
                Content = JsonContent.Create(new
                {
                    nome = "Mel",
                    especie = "Cachorro",
                    userId
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
            Assert.Equal("Mel", payload.GetProperty("data").GetProperty("nome").GetString());
            Assert.True(payload.TryGetProperty("_links", out _));
        }

        [Fact]
        public async Task GetPets_QuandoComDados_RetornaOkPaginadoComLinks()
        {
            // Arrange
            var email = $"listpet_{Guid.NewGuid():N}@email.com";
            var userId = await CreateUserAndGetIdAsync(email, "senha123");
            var token = await LoginAndGetTokenAsync(email, "senha123");
            await CreatePetAsync(token, userId, "Bob");

            // Act
            var response = await _client.GetAsync("/api/pets?page=1&pageSize=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("items", body, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("_links", body, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<int> CreateUserAndGetIdAsync(string email, string senha)
        {
            var response = await _client.PostAsJsonAsync("/api/users", new CreateUserDto
            {
                Nome = "Tutor Pet",
                Email = email,
                Senha = senha,
                TipoUsuario = "TUTOR"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
            return payload.GetProperty("data").GetProperty("id").GetInt32();
        }

        private async Task CreatePetAsync(string token, int userId, string nome)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/pets")
            {
                Content = JsonContent.Create(new
                {
                    nome,
                    especie = "Cachorro",
                    userId
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        private async Task<string> LoginAndGetTokenAsync(string email, string senha)
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Email = email,
                Senha = senha
            });

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);
            return body!.Token;
        }
    }
}
