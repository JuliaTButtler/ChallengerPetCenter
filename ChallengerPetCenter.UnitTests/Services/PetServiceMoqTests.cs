using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Rules;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Helpers;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class PetServiceMoqTests
    {
        private readonly Mock<IPetRepository> _pets = new();
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _uow = ServiceMocks.UnitOfWork();
        private readonly Mock<IAuditLogService> _audit = ServiceMocks.Audit();

        private PetService CreateSut() =>
            new(_pets.Object, _users.Object, _uow.Object, _audit.Object);

        [Fact]
        public async Task CreateAsync_QuandoTutorExiste_RetornaSuccess()
        {
            // Arrange
            _users.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _pets.Setup(r => r.AddAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask)
                .Callback<Pet>(p => p.Id = 5);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreatePetDto
            {
                Nome = "Mel",
                Especie = "Cachorro",
                UserId = 1
            });

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Mel", result.Data!.Nome);
        }

        [Fact]
        public async Task CreateAsync_QuandoPetSemTutor_RetornaFailure()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreatePetDto
            {
                Nome = "Mel",
                Especie = "Cachorro",
                UserId = 0
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(PetRules.MissingTutorMessage, result.Error);
            _pets.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_RetornaNotFound()
        {
            // Arrange
            _pets.Setup(r => r.GetByIdAsync(9)).ReturnsAsync((Pet?)null);
            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(9);

            // Assert
            Assert.True(result.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_QuandoExiste_RetornaSuccess()
        {
            // Arrange
            var pet = new Pet { Id = 2, Nome = "Bob", Especie = "Gato", UserId = 1 };
            _pets.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(pet);
            var sut = CreateSut();

            // Act
            var result = await sut.DeleteAsync(2);

            // Assert
            Assert.True(result.Succeeded);
            _pets.Verify(r => r.Remove(pet), Times.Once);
        }
    }
}
