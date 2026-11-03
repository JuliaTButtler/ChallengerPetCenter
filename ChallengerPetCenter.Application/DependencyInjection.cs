using ChallengerPetCenter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengerPetCenter.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IVeterinarioService, VeterinarioService>();
            services.AddScoped<IPetVeterinarioService, PetVeterinarioService>();
            services.AddScoped<IDiarioEntradaService, DiarioEntradaService>();
            services.AddScoped<IRegistroService, RegistroService>();
            services.AddScoped<IComentarioService, ComentarioService>();
            services.AddScoped<ISolicitacaoService, SolicitacaoService>();
            // IAuditLogService é registrado na Infrastructure (Mongo ou NoOp)

            return services;
        }
    }
}
