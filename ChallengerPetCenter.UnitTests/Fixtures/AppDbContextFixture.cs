using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Configuration;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Infrastructure.Repositories;
using ChallengerPetCenter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

        public ServiceBundle CreateServices(AppDbContext context)
        {
            var users = new UserRepository(context);
            var pets = new PetRepository(context);
            var veterinarios = new VeterinarioRepository(context);
            var petVeterinarios = new PetVeterinarioRepository(context);
            var diarioEntradas = new DiarioEntradaRepository(context);
            var registros = new RegistroRepository(context);
            var comentarios = new ComentarioRepository(context);
            var solicitacoes = new SolicitacaoRepository(context);
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            IAuditLogService audit = new NoOpAuditLogService();

            return new ServiceBundle(
                new UserService(users, unitOfWork, audit),
                new PetService(pets, users, unitOfWork, audit),
                new VeterinarioService(veterinarios, users, unitOfWork, audit),
                new PetVeterinarioService(petVeterinarios, pets, veterinarios, unitOfWork, audit),
                new DiarioEntradaService(diarioEntradas, pets, unitOfWork, audit),
                new RegistroService(registros, diarioEntradas, unitOfWork, audit),
                new ComentarioService(comentarios, diarioEntradas, users, unitOfWork, audit),
                new SolicitacaoService(solicitacoes, pets, users, veterinarios, unitOfWork, audit),
                new AuthService(
                    users,
                    unitOfWork,
                    Options.Create(new JwtSettings
                    {
                        Key = "ChallengerPetCenter_Test_Secret_Key_Min32Chars!",
                        Issuer = "ChallengerPetCenter",
                        Audience = "ChallengerPetCenter",
                        ExpirationMinutes = 60
                    })));
        }
    }

    public sealed record ServiceBundle(
        UserService Users,
        PetService Pets,
        VeterinarioService Veterinarios,
        PetVeterinarioService PetVeterinarios,
        DiarioEntradaService DiarioEntradas,
        RegistroService Registros,
        ComentarioService Comentarios,
        SolicitacaoService Solicitacoes,
        AuthService Auth);

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<AppDbContextFixture>
    {
        // Classe vazia: apenas registra a collection no xUnit.
    }
}
