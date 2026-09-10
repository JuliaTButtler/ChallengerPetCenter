using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Services
{
    [Collection("Database collection")]
    public class PetServiceTests
    {
        private readonly AppDbContextFixture _fixture;

        public PetServiceTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateAsync_QuandoTutorExiste_RetornaSuccess()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var tutor = new User
            {
                Nome = "Tutor",
                Email = $"tutor_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(tutor);
            await context.SaveChangesAsync();

            var service = new PetService(context);
            var dto = new CreatePetDto
            {
                Nome = "Mel",
                Especie = "Cachorro",
                UserId = tutor.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Mel", result.Data!.Nome);
            Assert.Equal(tutor.Id, result.Data.UserId);
        }

        [Fact]
        public async Task CreateAsync_QuandoTutorNaoExiste_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var service = new PetService(context);
            var dto = new CreatePetDto
            {
                Nome = "Mel",
                Especie = "Cachorro",
                UserId = 999
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Tutor não encontrado.", result.Error);
        }

        [Fact]
        public async Task GetByNomeAsync_QuandoNomeParcialExiste_RetornaPetsCorrespondentes()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var tutor = new User
            {
                Nome = "Tutor",
                Email = $"tutor_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(tutor);
            await context.SaveChangesAsync();

            context.Pets.AddRange(
                new Pet { Nome = "Bob", Especie = "Cachorro", UserId = tutor.Id },
                new Pet { Nome = "Bobby", Especie = "Cachorro", UserId = tutor.Id },
                new Pet { Nome = "Luna", Especie = "Gato", UserId = tutor.Id }
            );
            await context.SaveChangesAsync();

            var service = new PetService(context);

            // Act
            var result = (await service.GetByNomeAsync("bob")).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Contains("bob", p.Nome, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task DeleteAsync_QuandoPetNaoExiste_RetornaNotFound()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var service = new PetService(context);

            // Act
            var result = await service.DeleteAsync(123);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.NotFound);
        }
    }
}
