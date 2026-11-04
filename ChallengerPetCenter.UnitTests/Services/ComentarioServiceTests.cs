using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Helpers;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class ComentarioServiceTests
    {
        private readonly Mock<IComentarioRepository> _comentarios = new();
        private readonly Mock<IDiarioEntradaRepository> _diario = new();
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _uow = ServiceMocks.UnitOfWork();
        private readonly Mock<IAuditLogService> _audit = ServiceMocks.Audit();

        private ComentarioService CreateSut() =>
            new(_comentarios.Object, _diario.Object, _users.Object, _uow.Object, _audit.Object);

        [Fact]
        public async Task CreateAsync_QuandoDadosValidos_RetornaSuccess()
        {
            // Arrange
            _diario.Setup(r => r.ExistsAsync(10)).ReturnsAsync(true);
            _users.Setup(r => r.ExistsAsync(2)).ReturnsAsync(true);
            _comentarios.Setup(r => r.AddAsync(It.IsAny<Comentario>())).Returns(Task.CompletedTask)
                .Callback<Comentario>(c => c.Id = 11);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateComentarioDto
            {
                TextoComentario = "Observação clínica",
                DiarioEntradaId = 10,
                UserId = 2
            });

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(11, result.Data!.Id);
            Assert.Equal("Observação clínica", result.Data.TextoComentario);
        }

        [Fact]
        public async Task CreateAsync_QuandoDiarioNaoExiste_RetornaFailure()
        {
            // Arrange
            _diario.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);
            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(new CreateComentarioDto
            {
                TextoComentario = "x",
                DiarioEntradaId = 99,
                UserId = 1
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Entrada do diário não encontrada.", result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_RetornaNotFound()
        {
            // Arrange
            _comentarios.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Comentario?)null);
            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(5);

            // Assert
            Assert.True(result.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_QuandoExiste_RetornaSuccess()
        {
            // Arrange
            var comentario = new Comentario
            {
                Id = 4,
                TextoComentario = "ok",
                DiarioEntradaId = 1,
                UserId = 1
            };
            _comentarios.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(comentario);
            var sut = CreateSut();

            // Act
            var result = await sut.DeleteAsync(4);

            // Assert
            Assert.True(result.Succeeded);
            _comentarios.Verify(r => r.Remove(comentario), Times.Once);
        }
    }
}
