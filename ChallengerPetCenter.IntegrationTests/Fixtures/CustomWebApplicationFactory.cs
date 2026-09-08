using ChallengerPetCenter.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChallengerPetCenter.IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string TestApiKey = "test-api-key-sprint3";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["UseInMemoryDatabase"] = "true",
                    ["ApiSettings:ApiKey"] = TestApiKey,
                    ["ExternalServices:HealthCheckUrl"] = "",
                    ["ConnectionStrings:OracleConnection"] = "User Id=test;Password=test;Data Source=localhost:1521/XEPDB1;"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("PetCenterIntegrationTests"));
            });
        }
    }
}
