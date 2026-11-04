using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Rules;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Helpers;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class UserServiceMoqTests
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _uow = ServiceMocks.UnitOfWork();
        private readonly Mock<IAuditLogService> _audit = ServiceMocks.Audit();

        private UserService CreateSut() => new(_users.Object, _uow.Object, _audit.Object);

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_RetornaSuccess()
        {
            // Arrange
            _users.Setup(r => r.ExistsByEmailAsync("ana@email.com")).ReturnsAsync(false);
            _users.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask)
                .Callback<User>(u => u.Id = 7);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateUserDto
            {
                Nome = "Ana",
                Email = "ana@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            });

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(7, result.Data!.Id);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_QuandoEmailInvalido_RetornaFailure()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateUserDto
            {
                Nome = "Ana",
                Email = "invalido",
                Senha = "123",
                TipoUsuario = "TUTOR"
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(EmailRules.InvalidMessage, result.Error);
            _users.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_RetornaNotFound()
        {
            // Arrange
            _users.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);
            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(99);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_QuandoExiste_RetornaSuccess()
        {
            // Arrange
            var user = new User { Id = 3, Email = "a@email.com", Nome = "A", Senha = "1", TipoUsuario = "TUTOR" };
            _users.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
            var sut = CreateSut();

            // Act
            var result = await sut.DeleteAsync(3);

            // Assert
            Assert.True(result.Succeeded);
            _users.Verify(r => r.Remove(user), Times.Once);
        }
    }
}
