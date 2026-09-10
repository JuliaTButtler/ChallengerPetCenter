using ChallengerPetCenter.DTOs.Veterinarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Services
{
    [Collection("Database collection")]
    public class VeterinarioServiceTests
    {
        private readonly AppDbContextFixture _fixture;

        public VeterinarioServiceTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateAsync_QuandoCrmvJaExiste_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var user1 = new User
            {
                Nome = "Vet1",
                Email = $"v1_{Guid.NewGuid():N}@email.com",
                Senha = "1",
                TipoUsuario = "VETERINARIO"
            };
            var user2 = new User
            {
                Nome = "Vet2",
                Email = $"v2_{Guid.NewGuid():N}@email.com",
                Senha = "1",
                TipoUsuario = "VETERINARIO"
            };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            context.Veterinarios.Add(new Veterinario
            {
                Crmv = "SP-12345",
                UserId = user1.Id
            });
            await context.SaveChangesAsync();

            var service = new VeterinarioService(context);
            var dto = new CreateVeterinarioDto
            {
                Crmv = "SP-12345",
                UserId = user2.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("CRMV já cadastrado.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_QuandoUserJaVinculado_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var user = new User
            {
                Nome = "Vet",
                Email = $"vet_{Guid.NewGuid():N}@email.com",
                Senha = "1",
                TipoUsuario = "VETERINARIO"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.Veterinarios.Add(new Veterinario
            {
                Crmv = "SP-111",
                UserId = user.Id
            });
            await context.SaveChangesAsync();

            var service = new VeterinarioService(context);
            var dto = new CreateVeterinarioDto
            {
                Crmv = "SP-222",
                UserId = user.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Este usuário já está vinculado a um veterinário.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_RetornaSuccess()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var user = new User
            {
                Nome = "Vet",
                Email = $"vet_{Guid.NewGuid():N}@email.com",
                Senha = "1",
                TipoUsuario = "VETERINARIO"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new VeterinarioService(context);
            var dto = new CreateVeterinarioDto
            {
                Crmv = "SP-99999",
                Especialidade = "Clínica Geral",
                UserId = user.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("SP-99999", result.Data!.Crmv);
            Assert.Equal(1, await context.Veterinarios.CountAsync());
        }
    }
}
