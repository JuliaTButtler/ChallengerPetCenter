using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Services
{
    [Collection("Database collection")]
    public class UserServiceTests
    {
        private readonly AppDbContextFixture _fixture;

        public UserServiceTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_RetornaSuccessComUsuario()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var service = _fixture.CreateServices(context).Users;
            var dto = new CreateUserDto
            {
                Nome = "Ana Tutor",
                Email = "ana@email.com",
                Senha = "123456",
                TipoUsuario = "TUTOR"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Ana Tutor", result.Data!.Nome);
            Assert.Equal("ana@email.com", result.Data.Email);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task CreateAsync_QuandoEmailJaExiste_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            context.Users.Add(new User
            {
                Nome = "Existente",
                Email = "duplicado@email.com",
                Senha = "abc",
                TipoUsuario = "TUTOR"
            });
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).Users;
            var dto = new CreateUserDto
            {
                Nome = "Novo",
                Email = "duplicado@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email já cadastrado.", result.Error);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task GetByIdAsync_QuandoUsuarioNaoExiste_RetornaNotFound()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var service = _fixture.CreateServices(context).Users;

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.NotFound);
            Assert.Equal("Usuário não encontrado.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_QuandoEmailDeOutroUsuario_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            context.Users.AddRange(
                new User { Nome = "A", Email = "a@email.com", Senha = "1", TipoUsuario = "TUTOR" },
                new User { Nome = "B", Email = "b@email.com", Senha = "1", TipoUsuario = "TUTOR" }
            );
            await context.SaveChangesAsync();

            var userB = context.Users.Single(u => u.Email == "b@email.com");
            var service = _fixture.CreateServices(context).Users;
            var dto = new UpdateUserDto
            {
                Id = userB.Id,
                Nome = "B",
                Email = "a@email.com",
                TipoUsuario = "TUTOR",
                Ativo = 1
            };

            // Act
            var result = await service.UpdateAsync(userB.Id, dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email já cadastrado.", result.Error);
        }

        [Fact]
        public async Task DeleteAsync_QuandoUsuarioExiste_RemoveComSuccess()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var user = new User
            {
                Nome = "Remover",
                Email = "remover@email.com",
                Senha = "1",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).Users;

            // Act
            var result = await service.DeleteAsync(user.Id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Empty(context.Users);
        }
    }
}
