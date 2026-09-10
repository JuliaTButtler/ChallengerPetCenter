using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Services
{
    [Collection("Database collection")]
    public class PetVeterinarioServiceTests
    {
        private readonly AppDbContextFixture _fixture;

        public PetVeterinarioServiceTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateAsync_QuandoPetEVeterinarioExistem_RetornaSuccess()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, veterinario) = await SeedPetEVeterinarioAsync(context);
            var service = new PetVeterinarioService(context);
            var dto = new CreatePetVeterinarioDto
            {
                PetId = pet.Id,
                VeterinarioId = veterinario.Id,
                Observacoes = "Acompanhamento contínuo"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data!.Ativo);
            Assert.Equal(pet.Id, result.Data.PetId);
            Assert.Equal(veterinario.Id, result.Data.VeterinarioId);
        }

        [Fact]
        public async Task CreateAsync_QuandoJaExisteVinculoAtivo_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, veterinario) = await SeedPetEVeterinarioAsync(context);

            context.PetVeterinarios.Add(new PetVeterinario
            {
                PetId = pet.Id,
                VeterinarioId = veterinario.Id,
                Ativo = 1
            });
            await context.SaveChangesAsync();

            var service = new PetVeterinarioService(context);
            var dto = new CreatePetVeterinarioDto
            {
                PetId = pet.Id,
                VeterinarioId = veterinario.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Já existe um vínculo ativo entre este pet e veterinário.",
                result.Error);
        }

        [Fact]
        public async Task CreateAsync_QuandoPetNaoExiste_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (_, veterinario) = await SeedPetEVeterinarioAsync(context);
            var service = new PetVeterinarioService(context);
            var dto = new CreatePetVeterinarioDto
            {
                PetId = 999,
                VeterinarioId = veterinario.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Pet não encontrado.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_QuandoReativaComOutroAtivoExistente_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, veterinario) = await SeedPetEVeterinarioAsync(context);

            var inativo = new PetVeterinario
            {
                PetId = pet.Id,
                VeterinarioId = veterinario.Id,
                Ativo = 0
            };
            var ativo = new PetVeterinario
            {
                PetId = pet.Id,
                VeterinarioId = veterinario.Id,
                Ativo = 1
            };
            context.PetVeterinarios.AddRange(inativo, ativo);
            await context.SaveChangesAsync();

            var service = new PetVeterinarioService(context);
            var dto = new UpdatePetVeterinarioDto
            {
                Id = inativo.Id,
                Ativo = 1,
                Observacoes = null
            };

            // Act
            var result = await service.UpdateAsync(inativo.Id, dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Já existe um vínculo ativo entre este pet e veterinário.",
                result.Error);
        }

        [Fact]
        public async Task GetAtivosAsync_QuandoExistemAtivosEInativos_RetornaSomenteAtivos()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, veterinario) = await SeedPetEVeterinarioAsync(context);

            context.PetVeterinarios.AddRange(
                new PetVeterinario { PetId = pet.Id, VeterinarioId = veterinario.Id, Ativo = 1 },
                new PetVeterinario { PetId = pet.Id, VeterinarioId = veterinario.Id, Ativo = 0 }
            );
            await context.SaveChangesAsync();

            var service = new PetVeterinarioService(context);

            // Act
            var result = (await service.GetAtivosAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Ativo);
        }

        private static async Task<(Pet Pet, Veterinario Veterinario)> SeedPetEVeterinarioAsync(
            Data.AppDbContext context)
        {
            var tutor = new User
            {
                Nome = "Tutor",
                Email = $"tutor_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            var vetUser = new User
            {
                Nome = "Vet",
                Email = $"vet_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "VETERINARIO"
            };
            context.Users.AddRange(tutor, vetUser);
            await context.SaveChangesAsync();

            var pet = new Pet { Nome = "Luna", Especie = "Gato", UserId = tutor.Id };
            var veterinario = new Veterinario
            {
                Crmv = $"CRMV-{Guid.NewGuid():N}"[..20],
                UserId = vetUser.Id
            };
            context.Pets.Add(pet);
            context.Veterinarios.Add(veterinario);
            await context.SaveChangesAsync();

            return (pet, veterinario);
        }
    }
}
