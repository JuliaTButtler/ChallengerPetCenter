using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengerPetCenter.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(
                    configuration.GetConnectionString("OracleConnection"),
                    oracle => oracle.MigrationsAssembly(
                        typeof(AppDbContext).Assembly.GetName().Name)));

            services.AddScoped<IAppDbContext>(provider =>
                provider.GetRequiredService<AppDbContext>());

            return services;
        }
    }
}
