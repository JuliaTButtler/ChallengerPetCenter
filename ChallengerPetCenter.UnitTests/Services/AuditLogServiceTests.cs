using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class AuditLogServiceTests
    {
        [Fact]
        public async Task WriteAsync_QuandoRepositorioFalha_NaoPropagaExcecao()
        {
            // Arrange
            var repo = new Mock<IAuditLogRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("mongo down"));

            var service = new AuditLogService(repo.Object, NullLogger<AuditLogService>.Instance);

            // Act
            var exception = await Record.ExceptionAsync(() =>
                service.WriteAsync("USER_CREATED", "User", "1", new { Nome = "Ana" }));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task QueryAsync_QuandoHaItens_RetornaPaginado()
        {
            // Arrange
            var repo = new Mock<IAuditLogRepository>();
            repo.Setup(r => r.QueryAsync("User", null, null, 1, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync((
                    new List<AuditLog>
                    {
                        new()
                        {
                            Id = "abc",
                            Action = "USER_CREATED",
                            Entity = "User",
                            EntityId = "1",
                            OccurredAt = DateTime.UtcNow,
                            PayloadJson = "{\"nome\":\"Ana\"}"
                        }
                    },
                    1L));

            var service = new AuditLogService(repo.Object, NullLogger<AuditLogService>.Instance);

            // Act
            var result = await service.QueryAsync(entity: "User");

            // Assert
            Assert.Equal(1, result.Total);
            Assert.Single(result.Items);
            Assert.Equal("USER_CREATED", result.Items[0].Action);
            Assert.Equal("User", result.Items[0].Entity);
        }
    }
}
