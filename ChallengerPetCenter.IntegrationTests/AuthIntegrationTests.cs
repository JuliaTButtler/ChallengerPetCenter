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
    public class AuthIntegrationTests
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Me_QuandoSemToken_RetornaUnauthorized()
        {
            // Arrange / Act
            var response = await _client.GetAsync("/api/auth/me");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_QuandoCredenciaisValidas_RetornaToken()
        {
            // Arrange
            var email = $"auth_{Guid.NewGuid():N}@email.com";
            await CreateUserAsync(email, "senha123");

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Email = email,
                Senha = "senha123"
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);
            Assert.NotNull(body);
            Assert.False(string.IsNullOrWhiteSpace(body!.Token));
            Assert.Equal(email, body.Email);
        }

        [Fact]
        public async Task Login_QuandoSenhaInvalida_RetornaBadRequest()
        {
            // Arrange
            var email = $"auth_{Guid.NewGuid():N}@email.com";
            await CreateUserAsync(email, "senha123");

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Email = email,
                Senha = "errada"
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Me_QuandoComTokenValido_RetornaOk()
        {
            // Arrange
            var email = $"auth_{Guid.NewGuid():N}@email.com";
            await CreateUserAsync(email, "senha123");
            var token = await LoginAndGetTokenAsync(email, "senha123");

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
            Assert.Equal(email, payload.GetProperty("email").GetString());
        }

        [Fact]
        public async Task DeleteUser_QuandoSemToken_RetornaUnauthorized()
        {
            // Arrange / Act
            var response = await _client.DeleteAsync("/api/users/1");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_QuandoComToken_RetornaNoContent()
        {
            // Arrange
            var email = $"del_{Guid.NewGuid():N}@email.com";
            var createResponse = await CreateUserAsync(email, "senha123");
            var created = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>(JsonOptions);
            Assert.NotNull(created);

            var token = await LoginAndGetTokenAsync(email, "senha123");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/{created!.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CreateUserAsync(string email, string senha)
        {
            var response = await _client.PostAsJsonAsync("/api/users", new CreateUserDto
            {
                Nome = "Usuario Teste",
                Email = email,
                Senha = senha,
                TipoUsuario = "TUTOR"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return response;
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
            Assert.NotNull(body);
            return body!.Token;
        }
    }
}
