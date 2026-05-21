using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets

        public DbSet<User> Users { get; set; }

        public DbSet<Veterinario> Veterinarios { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<PetVeterinario> PetVeterinarios { get; set; }

        public DbSet<DiarioEntrada> DiarioEntradas { get; set; }

        public DbSet<Registro> Registros { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<Solicitacao> Solicitacoes { get; set; }

        // Configurações das entidades

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Nomes das tabelas

            modelBuilder.Entity<User>().ToTable("petdiary_users");

            modelBuilder.Entity<Veterinario>().ToTable("petdiary_veterinarios");

            modelBuilder.Entity<Pet>().ToTable("petdiary_pets");

            modelBuilder.Entity<PetVeterinario>().ToTable("petdiary_pet_veterinario");

            modelBuilder.Entity<DiarioEntrada>().ToTable("petdiary_diario_entradas");

            modelBuilder.Entity<Registro>().ToTable("petdiary_registros");

            modelBuilder.Entity<Comentario>().ToTable("petdiary_comentarios");

            modelBuilder.Entity<Solicitacao>().ToTable("petdiary_solicitacoes");

            // User
            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

            // Veterinario -> User
            modelBuilder.Entity<Veterinario>()
                .HasOne(v => v.User)
                .WithOne(u => u.Veterinario)
                .HasForeignKey<Veterinario>(v => v.UserId);

            // Pet -> User

            modelBuilder.Entity<Pet>()
                .HasOne(p => p.User)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.UserId);

            // DiarioEntrada -> Pet

            modelBuilder.Entity<DiarioEntrada>()
                .HasOne(d => d.Pet)
                .WithMany(p => p.DiarioEntradas)
                .HasForeignKey(d => d.PetId);

            // Registro -> DiarioEntrada
            modelBuilder.Entity<Registro>()
                .Property(r => r.Valor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Registro>()
                .HasOne(r => r.DiarioEntrada)
                .WithMany(d => d.Registros)
                .HasForeignKey(r => r.DiarioEntradaId);

            // Comentario -> DiarioEntrada

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.DiarioEntrada)
                .WithMany(d => d.Comentarios)
                .HasForeignKey(c => c.DiarioEntradaId);

            // Comentario -> User

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.UserId);

            // PetVeterinario -> Pet

            modelBuilder.Entity<PetVeterinario>()
                .HasOne(pv => pv.Pet)
                .WithMany(p => p.PetVeterinarios)
                .HasForeignKey(pv => pv.PetId);

            
            // PetVeterinario -> Veterinario

            modelBuilder.Entity<PetVeterinario>()
                .HasOne(pv => pv.Veterinario)
                .WithMany(v => v.PetVeterinarios)
                .HasForeignKey(pv => pv.VeterinarioId);

            // Solicitacao -> Pet

            modelBuilder.Entity<Solicitacao>()
                .HasOne(s => s.Pet)
                .WithMany(p => p.Solicitacoes)
                .HasForeignKey(s => s.PetId);

            // Solicitacao -> Tutor (User)

            modelBuilder.Entity<Solicitacao>()
                .HasOne(s => s.Tutor)
                .WithMany(u => u.Solicitacoes)
                .HasForeignKey(s => s.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Solicitacao -> Veterinario

            modelBuilder.Entity<Solicitacao>()
                .HasOne(s => s.Veterinario)
                .WithMany(v => v.Solicitacoes)
                .HasForeignKey(s => s.VeterinarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // UNIQUE (PetId + Data)

            modelBuilder.Entity<DiarioEntrada>()
                .HasIndex(d => new { d.PetId, d.Data })
                .IsUnique();
        }
    }
}