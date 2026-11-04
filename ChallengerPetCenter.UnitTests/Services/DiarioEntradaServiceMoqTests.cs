using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Rules;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Helpers;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class DiarioEntradaServiceMoqTests
    {
        private readonly Mock<IDiarioEntradaRepository> _diario = new();
        private readonly Mock<IPetRepository> _pets = new();
        private readonly Mock<IUnitOfWork> _uow = ServiceMocks.UnitOfWork();
        private readonly Mock<IAuditLogService> _audit = ServiceMocks.Audit();

        private DiarioEntradaService CreateSut() =>
            new(_diario.Object, _pets.Object, _uow.Object, _audit.Object);

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_RetornaSuccess()
        {
            // Arrange
            var data = new DateTime(2026, 11, 3);
            _pets.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _diario.Setup(r => r.ExistsByPetAndDateAsync(1, data, It.IsAny<int?>())).ReturnsAsync(false);
            _diario.Setup(r => r.AddAsync(It.IsAny<DiarioEntrada>())).Returns(Task.CompletedTask)
                .Callback<DiarioEntrada>(e => e.Id = 4);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateDiarioEntradaDto
            {
                PetId = 1,
                Data = data,
                Status = "ATIVO"
            });

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(4, result.Data!.Id);
        }

        [Fact]
        public async Task CreateAsync_QuandoDiarioDuplicadoNoMesmoDia_RetornaFailure()
        {
            // Arrange
            var data = new DateTime(2026, 11, 3);
            _pets.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _diario.Setup(r => r.ExistsByPetAndDateAsync(1, data, It.IsAny<int?>())).ReturnsAsync(true);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateDiarioEntradaDto
            {
                PetId = 1,
                Data = data
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(DiarioEntradaRules.DuplicateSameDayMessage, result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_RetornaNotFound()
        {
            // Arrange
            _diario.Setup(r => r.GetByIdAsync(8)).ReturnsAsync((DiarioEntrada?)null);
            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(8);

            // Assert
            Assert.True(result.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_QuandoExiste_RetornaSuccess()
        {
            // Arrange
            var entrada = new DiarioEntrada { Id = 3, PetId = 1, Data = DateTime.Today };
            _diario.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(entrada);
            var sut = CreateSut();

            // Act
            var result = await sut.DeleteAsync(3);

            // Assert
            Assert.True(result.Succeeded);
            _diario.Verify(r => r.Remove(entrada), Times.Once);
        }
    }
}
