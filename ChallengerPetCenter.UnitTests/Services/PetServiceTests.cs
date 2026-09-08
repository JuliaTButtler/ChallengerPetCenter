using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class PetServiceTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_QuandoTutorNaoExiste_DeveRetornarBadRequest()
        {
            // Arrange
            await using var context = CreateContext();
            var logger = new Mock<ILogger<PetService>>();
            var service = new PetService(context, logger.Object);
            var dto = new CreatePetDto
            {
                Nome = "Bob",
                Especie = "Cachorro",
                UserId = 99
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ServiceErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Tutor não encontrado.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_QuandoTutorExiste_DeveCriarPet()
        {
            // Arrange
            await using var context = CreateContext();
            var tutor = new User
            {
                Nome = "Tutor",
                Email = "tutor@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(tutor);
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<PetService>>();
            var service = new PetService(context, logger.Object);
            var dto = new CreatePetDto
            {
                Nome = "Bob",
                Especie = "Cachorro",
                UserId = tutor.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Bob", result.Data!.Nome);
            Assert.Equal(tutor.Id, result.Data.UserId);
        }

        [Fact]
        public async Task GetByUserAsync_QuandoNaoHaPets_DeveRetornarNotFound()
        {
            // Arrange
            await using var context = CreateContext();
            var logger = new Mock<ILogger<PetService>>();
            var service = new PetService(context, logger.Object);

            // Act
            var result = await service.GetByUserAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
        }
    }
}
