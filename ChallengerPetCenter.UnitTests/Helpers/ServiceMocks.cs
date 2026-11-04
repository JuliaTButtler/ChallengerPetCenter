using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Services;
using Moq;

namespace ChallengerPetCenter.UnitTests.Helpers
{
    internal static class ServiceMocks
    {
        public static Mock<IUnitOfWork> UnitOfWork()
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            return mock;
        }

        public static Mock<IAuditLogService> Audit()
        {
            var mock = new Mock<IAuditLogService>();
            mock.Setup(a => a.WriteAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<object?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            return mock;
        }
    }
}
