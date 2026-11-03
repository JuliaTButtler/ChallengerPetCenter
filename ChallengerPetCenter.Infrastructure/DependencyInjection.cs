using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Infrastructure.Repositories;
using ChallengerPetCenter.Repositories;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();
            services.AddScoped<IPetVeterinarioRepository, PetVeterinarioRepository>();
            services.AddScoped<IDiarioEntradaRepository, DiarioEntradaRepository>();
            services.AddScoped<IRegistroRepository, RegistroRepository>();
            services.AddScoped<IComentarioRepository, ComentarioRepository>();
            services.AddScoped<ISolicitacaoRepository, SolicitacaoRepository>();

            return services;
        }
    }
}
