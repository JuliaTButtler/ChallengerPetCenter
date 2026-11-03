using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Infrastructure.Repositories;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ChallengerPetCenter.Configuration;

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

            return new ServiceBundle(
                new UserService(users, unitOfWork),
                new PetService(pets, users, unitOfWork),
                new VeterinarioService(veterinarios, users, unitOfWork),
                new PetVeterinarioService(petVeterinarios, pets, veterinarios, unitOfWork),
                new DiarioEntradaService(diarioEntradas, pets, unitOfWork),
                new RegistroService(registros, diarioEntradas, unitOfWork),
                new ComentarioService(comentarios, diarioEntradas, users, unitOfWork),
                new SolicitacaoService(solicitacoes, pets, users, veterinarios, unitOfWork),
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
