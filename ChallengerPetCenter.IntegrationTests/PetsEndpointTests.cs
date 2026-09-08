using System.Net;
using System.Net.Http.Json;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.IntegrationTests.Fixtures;
using ChallengerPetCenter.Middleware;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection(ApiCollection.Name)]
    public class PetsEndpointTests
    {
        private readonly HttpClient _client;

        public PetsEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add(ApiKeyMiddleware.HeaderName, CustomWebApplicationFactory.TestApiKey);
        }

        [Fact]
        public async Task CreatePet_QuandoTutorExiste_DeveRetornarCreated()
        {
            // Arrange
            var tutorResponse = await _client.PostAsJsonAsync("/api/users", new CreateUserDto
            {
                Nome = "Tutor Pet",
                Email = $"tutor-pet-{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            });
            tutorResponse.EnsureSuccessStatusCode();
            var tutor = await tutorResponse.Content.ReadFromJsonAsync<UserResponseDto>();

            var dto = new CreatePetDto
            {
                Nome = "Thor",
                Especie = "Cachorro",
                UserId = tutor!.Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/pets", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var pet = await response.Content.ReadFromJsonAsync<PetResponseDto>();
            Assert.NotNull(pet);
            Assert.Equal("Thor", pet!.Nome);
        }

        [Fact]
        public async Task CreatePet_QuandoTutorNaoExiste_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new CreatePetDto
            {
                Nome = "Fantasma",
                Especie = "Gato",
                UserId = 999999
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/pets", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetPetById_QuandoNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var id = 888888;

            // Act
            var response = await _client.GetAsync($"/api/pets/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
