using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.UnitTests.Services
{
    [Collection("Database collection")]
    public class SolicitacaoServiceTests
    {
        private readonly AppDbContextFixture _fixture;

        public SolicitacaoServiceTests(AppDbContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateAsync_QuandoTutorEhDonoDoPet_RetornaSuccessComStatusPendente()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, tutor, veterinario) = await SeedAsync(context);
            var service = _fixture.CreateServices(context).Solicitacoes;
            var dto = new CreateSolicitacaoDto
            {
                PetId = pet.Id,
                TutorId = tutor.Id,
                VeterinarioId = veterinario.Id,
                Mensagem = "Preciso de acompanhamento"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("PENDENTE", result.Data!.Status);
            Assert.Equal(pet.Id, result.Data.PetId);
        }

        [Fact]
        public async Task CreateAsync_QuandoTutorNaoEhDonoDoPet_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, _, veterinario) = await SeedAsync(context);

            var outroTutor = new User
            {
                Nome = "Outro",
                Email = $"outro_{Guid.NewGuid():N}@email.com",
                Senha = "1",
                TipoUsuario = "TUTOR"
            };
            context.Users.Add(outroTutor);
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).Solicitacoes;
            var dto = new CreateSolicitacaoDto
            {
                PetId = pet.Id,
                TutorId = outroTutor.Id,
                VeterinarioId = veterinario.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("O tutor informado não é o responsável por este pet.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_QuandoPetNaoExiste_RetornaFailure()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (_, tutor, veterinario) = await SeedAsync(context);
            var service = _fixture.CreateServices(context).Solicitacoes;
            var dto = new CreateSolicitacaoDto
            {
                PetId = 999,
                TutorId = tutor.Id,
                VeterinarioId = veterinario.Id
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Pet não encontrado.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_QuandoMudaDePendenteParaAceita_PreencheRespondidoEm()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, tutor, veterinario) = await SeedAsync(context);

            var solicitacao = new Solicitacao
            {
                PetId = pet.Id,
                TutorId = tutor.Id,
                VeterinarioId = veterinario.Id,
                Status = "PENDENTE",
                Mensagem = "Ajuda"
            };
            context.Solicitacoes.Add(solicitacao);
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).Solicitacoes;
            var dto = new UpdateSolicitacaoDto
            {
                Id = solicitacao.Id,
                Status = "ACEITA",
                Mensagem = "Aceito acompanhar"
            };

            // Act
            var result = await service.UpdateAsync(solicitacao.Id, dto);

            // Assert
            Assert.True(result.Succeeded);
            var atualizada = await context.Solicitacoes.FindAsync(solicitacao.Id);
            Assert.NotNull(atualizada);
            Assert.Equal("ACEITA", atualizada!.Status);
            Assert.NotNull(atualizada.RespondidoEm);
        }

        [Fact]
        public async Task GetByStatusAsync_QuandoFiltraPendente_RetornaSomentePendentes()
        {
            // Arrange
            await using var context = _fixture.CreateContext();
            var (pet, tutor, veterinario) = await SeedAsync(context);

            context.Solicitacoes.AddRange(
                new Solicitacao
                {
                    PetId = pet.Id,
                    TutorId = tutor.Id,
                    VeterinarioId = veterinario.Id,
                    Status = "PENDENTE"
                },
                new Solicitacao
                {
                    PetId = pet.Id,
                    TutorId = tutor.Id,
                    VeterinarioId = veterinario.Id,
                    Status = "ACEITA"
                }
            );
            await context.SaveChangesAsync();

            var service = _fixture.CreateServices(context).Solicitacoes;

            // Act
            var result = (await service.GetByStatusAsync("pendente")).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("PENDENTE", result[0].Status);
        }

        private static async Task<(Pet Pet, User Tutor, Veterinario Veterinario)> SeedAsync(
            AppDbContext context)
        {
            var tutor = new User
            {
                Nome = "Tutor",
                Email = $"tutor_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };
            var vetUser = new User
            {
                Nome = "Vet",
                Email = $"vet_{Guid.NewGuid():N}@email.com",
                Senha = "123",
                TipoUsuario = "VETERINARIO"
            };
            context.Users.AddRange(tutor, vetUser);
            await context.SaveChangesAsync();

            var pet = new Pet { Nome = "Thor", Especie = "Cachorro", UserId = tutor.Id };
            var veterinario = new Veterinario
            {
                Crmv = $"CRMV-{Guid.NewGuid():N}"[..20],
                UserId = vetUser.Id
            };
            context.Pets.Add(pet);
            context.Veterinarios.Add(veterinario);
            await context.SaveChangesAsync();

            return (pet, tutor, veterinario);
        }
    }
}
