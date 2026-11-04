using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Helpers;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class RegistroServiceTests
    {
        private readonly Mock<IRegistroRepository> _registros = new();
        private readonly Mock<IDiarioEntradaRepository> _diario = new();
        private readonly Mock<IUnitOfWork> _uow = ServiceMocks.UnitOfWork();
        private readonly Mock<IAuditLogService> _audit = ServiceMocks.Audit();

        private RegistroService CreateSut() =>
            new(_registros.Object, _diario.Object, _uow.Object, _audit.Object);

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_RetornaSuccess()
        {
            // Arrange
            _diario.Setup(r => r.ExistsAsync(3)).ReturnsAsync(true);
            _registros.Setup(r => r.AddAsync(It.IsAny<Registro>())).Returns(Task.CompletedTask)
                .Callback<Registro>(r => r.Id = 6);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateRegistroDto
            {
                Tipo = "ALIMENTACAO",
                Subtipo = "RACAO",
                Valor = 120,
                Unidade = "g",
                DiarioEntradaId = 3
            });

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(6, result.Data!.Id);
            Assert.Equal("ALIMENTACAO", result.Data.Tipo);
        }

        [Fact]
        public async Task CreateAsync_QuandoDiarioNaoExiste_RetornaFailure()
        {
            // Arrange
            _diario.Setup(r => r.ExistsAsync(50)).ReturnsAsync(false);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateRegistroDto
            {
                Tipo = "SINTOMA",
                DiarioEntradaId = 50
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Entrada do diário não encontrada.", result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_RetornaNotFound()
        {
            // Arrange
            _registros.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Registro?)null);
            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(1);

            // Assert
            Assert.True(result.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_QuandoExiste_RetornaSuccess()
        {
            // Arrange
            var registro = new Registro { Id = 9, Tipo = "ATIVIDADE", DiarioEntradaId = 1 };
            _registros.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(registro);
            var sut = CreateSut();

            // Act
            var result = await sut.DeleteAsync(9);

            // Assert
            Assert.True(result.Succeeded);
            _registros.Verify(r => r.Remove(registro), Times.Once);
        }
    }
}
