using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Services
{
    [Collection("Database collection")]
    public class DiarioEntradaServiceTests
    {
        private readonly AppDbContextFixture _fixture;

        public DiarioEntradaServiceTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateAsync_QuandoPetExisteEDataLivre_RetornaSuccess()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var pet = await SeedPetAsync(context);
            var service = _fixture.CreateServices(context).DiarioEntradas;
            var dto = new CreateDiarioEntradaDto
            {
                PetId = pet.Id,
                Data = new DateTime(2026, 9, 10),
                HumorGeral = "FELIZ",
                Status = "ATIVO",
                Resumo = "Dia tranquilo"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(pet.Id, result.Data!.PetId);
            Assert.Equal(new DateTime(2026, 9, 10), result.Data.Data.Date);
        }

        [Fact]
        public async Task CreateAsync_QuandoJaExisteEntradaNaMesmaData_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var pet = await SeedPetAsync(context);
            var data = new DateTime(2026, 9, 10);

            context.DiarioEntradas.Add(new DiarioEntrada
            {
                PetId = pet.Id,
                Data = data,
                Status = "ATIVO"
            });
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).DiarioEntradas;
            var dto = new CreateDiarioEntradaDto
            {
                PetId = pet.Id,
                Data = data,
                Status = "ATIVO"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Já existe uma entrada para este pet nesta data.", result.Error);
            Assert.Equal(1, await context.DiarioEntradas.CountAsync());
        }

        [Fact]
        public async Task CreateAsync_QuandoPetNaoExiste_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var service = _fixture.CreateServices(context).DiarioEntradas;
            var dto = new CreateDiarioEntradaDto
            {
                PetId = 999,
                Data = DateTime.Today
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Pet não encontrado.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_QuandoNovaDataJaOcupada_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var pet = await SeedPetAsync(context);

            var entrada1 = new DiarioEntrada { PetId = pet.Id, Data = new DateTime(2026, 9, 1) };
            var entrada2 = new DiarioEntrada { PetId = pet.Id, Data = new DateTime(2026, 9, 2) };
            context.DiarioEntradas.AddRange(entrada1, entrada2);
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).DiarioEntradas;
            var dto = new UpdateDiarioEntradaDto
            {
                Id = entrada2.Id,
                Data = new DateTime(2026, 9, 1),
                Status = "ATIVO"
            };

            // Act
            var result = await service.UpdateAsync(entrada2.Id, dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Já existe uma entrada para este pet nesta data.", result.Error);
        }

        [Fact]
        public async Task GetByPetAsync_QuandoExistemEntradas_RetornaOrdenadoPorDataDesc()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var pet = await SeedPetAsync(context);
            context.DiarioEntradas.AddRange(
                new DiarioEntrada { PetId = pet.Id, Data = new DateTime(2026, 9, 1) },
                new DiarioEntrada { PetId = pet.Id, Data = new DateTime(2026, 9, 3) },
                new DiarioEntrada { PetId = pet.Id, Data = new DateTime(2026, 9, 2) }
            );
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).DiarioEntradas;

            // Act
            var result = (await service.GetByPetAsync(pet.Id)).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(new DateTime(2026, 9, 3), result[0].Data.Date);
            Assert.Equal(new DateTime(2026, 9, 2), result[1].Data.Date);
            Assert.Equal(new DateTime(2026, 9, 1), result[2].Data.Date);
        }

        private static async Task<Pet> SeedPetAsync(AppDbContext context)
        {
            var user = new User
            {
                Nome = "Tutor",
                Email = $"tutor_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var pet = new Pet
            {
                Nome = "Bob",
                Especie = "Cachorro",
                UserId = user.Id
            };
            context.Pets.Add(pet);
            await context.SaveChangesAsync();
            return pet;
        }
    }
}
