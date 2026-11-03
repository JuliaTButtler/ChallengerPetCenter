using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Abstractions
{
    /// <summary>
    /// Abstração de persistência relacional para a camada de Application.
    /// Implementação concreta (EF Core) fica em Infrastructure.
    /// </summary>
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Veterinario> Veterinarios { get; }
        DbSet<Pet> Pets { get; }
        DbSet<PetVeterinario> PetVeterinarios { get; }
        DbSet<DiarioEntrada> DiarioEntradas { get; }
        DbSet<Registro> Registros { get; }
        DbSet<Comentario> Comentarios { get; }
        DbSet<Solicitacao> Solicitacoes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
