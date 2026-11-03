using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Infrastructure.HealthChecks;
using ChallengerPetCenter.Infrastructure.Mongo;
using ChallengerPetCenter.Infrastructure.Repositories;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

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

            AddMongo(services, configuration);

            return services;
        }

        private static void AddMongo(IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnection = configuration.GetConnectionString("MongoConnection");
            if (string.IsNullOrWhiteSpace(mongoConnection))
            {
                services.AddSingleton<IAuditLogService, NoOpAuditLogService>();
                return;
            }

            var mongoUrl = new MongoUrl(mongoConnection);
            var databaseName = string.IsNullOrWhiteSpace(mongoUrl.DatabaseName)
                ? "PetCenter"
                : mongoUrl.DatabaseName;

            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = mongoConnection;
                options.DatabaseName = databaseName;
                options.AuditLogsCollection = "audit_logs";
            });

            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoUrl));
            services.AddScoped<IAuditLogRepository, MongoAuditLogRepository>();
            services.AddScoped<IAuditLogService, AuditLogService>();

            services.AddHealthChecks()
                .AddCheck<MongoDbHealthCheck>(
                    name: "mongodb",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "database", "ready", "mongodb" });
        }
    }
}
