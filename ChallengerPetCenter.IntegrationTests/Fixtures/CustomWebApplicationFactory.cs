using ChallengerPetCenter.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChallengerPetCenter.IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"PetCenterIntegration_{Guid.NewGuid():N}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));

                var efOptionsDescriptors = services
                    .Where(d =>
                        d.ServiceType.FullName is not null &&
                        d.ServiceType.FullName.Contains("DbContextOptions", StringComparison.Ordinal) &&
                        d.ServiceType.FullName.Contains(nameof(AppDbContext), StringComparison.Ordinal))
                    .ToList();

                foreach (var descriptor in efOptionsDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }
    }

    [CollectionDefinition("Integration collection")]
    public class IntegrationCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
        // Marca a collection compartilhada no xUnit.
    }
}
