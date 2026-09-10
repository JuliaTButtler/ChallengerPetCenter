using ChallengerPetCenter.Data;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Fixtures
{
    /// <summary>
    /// Factory compartilhada via Collection Fixture.
    /// Cada chamada cria um banco InMemory isolado (sem contaminação entre testes).
    /// </summary>
    public class AppDbContextFixture
    {
        public AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"PetCenterTests_{Guid.NewGuid()}")
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<AppDbContextFixture>
    {
        // Classe vazia: apenas registra a collection no xUnit.
    }
}
