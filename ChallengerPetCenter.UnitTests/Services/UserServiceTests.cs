using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class UserServiceTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_QuandoEmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            await using var context = CreateContext();
            context.Users.Add(new User
            {
                Nome = "Existente",
                Email = "existente@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            });
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<UserService>>();
            var service = new UserService(context, logger.Object);
            var dto = new CreateUserDto
            {
                Nome = "Novo",
                Email = "existente@email.com",
                Senha = "abc",
                TipoUsuario = "TUTOR"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ServiceErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Email já cadastrado.", result.Message);
            logger.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_DeveCriarUsuario()
        {
            // Arrange
            await using var context = CreateContext();
            var logger = new Mock<ILogger<UserService>>();
            var service = new UserService(context, logger.Object);
            var dto = new CreateUserDto
            {
                Nome = "Ana",
                Email = "ana@email.com",
                Senha = "senha",
                TipoUsuario = "TUTOR"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("TUTOR", result.Data!.TipoUsuario);
            Assert.Equal("ana@email.com", result.Data.Email);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            await using var context = CreateContext();
            var logger = new Mock<ILogger<UserService>>();
            var service = new UserService(context, logger.Object);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
            Assert.Equal("Usuário não encontrado.", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_QuandoExiste_DeveRemoverUsuario()
        {
            // Arrange
            await using var context = CreateContext();
            var user = new User
            {
                Nome = "Remover",
                Email = "remover@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<UserService>>();
            var service = new UserService(context, logger.Object);

            // Act
            var result = await service.DeleteAsync(user.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Empty(context.Users);
        }
    }
}
