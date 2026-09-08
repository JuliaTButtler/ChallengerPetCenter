using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class DiarioEntradaServiceTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static async Task<Pet> SeedPetAsync(AppDbContext context)
        {
            var user = new User
            {
                Nome = "Tutor",
                Email = $"tutor-{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var pet = new Pet
            {
                Nome = "Luna",
                Especie = "Gato",
                UserId = user.Id
            };
            context.Pets.Add(pet);
            await context.SaveChangesAsync();
            return pet;
        }

        [Fact]
        public async Task CreateAsync_QuandoJaExisteEntradaNaData_DeveRetornarBadRequest()
        {
            // Arrange
            await using var context = CreateContext();
            var pet = await SeedPetAsync(context);
            var data = DateTime.Today;

            context.DiarioEntradas.Add(new DiarioEntrada
            {
                PetId = pet.Id,
                Data = data,
                Status = "ATIVO"
            });
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<DiarioEntradaService>>();
            var service = new DiarioEntradaService(context, logger.Object);
            var dto = new CreateDiarioEntradaDto
            {
                PetId = pet.Id,
                Data = data,
                Status = "ATIVO"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ServiceErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Já existe uma entrada para este pet nesta data.", result.Message);
        }

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_DeveCriarEntrada()
        {
            // Arrange
            await using var context = CreateContext();
            var pet = await SeedPetAsync(context);
            var logger = new Mock<ILogger<DiarioEntradaService>>();
            var service = new DiarioEntradaService(context, logger.Object);
            var dto = new CreateDiarioEntradaDto
            {
                PetId = pet.Id,
                Data = DateTime.Today,
                HumorGeral = "FELIZ",
                Status = "ATIVO"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(pet.Id, result.Data!.PetId);
            Assert.Equal("FELIZ", result.Data.HumorGeral);
        }
    }
}
