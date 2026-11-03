using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class SolicitacaoRepository : ISolicitacaoRepository
    {
        private readonly AppDbContext _context;

        public SolicitacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Solicitacao>> GetAllAsync()
            => await _context.Solicitacoes.ToListAsync();

        public async Task<Solicitacao?> GetByIdAsync(int id)
            => await _context.Solicitacoes.FindAsync(id);

        public async Task<IReadOnlyList<Solicitacao>> GetByPetAsync(int petId)
            => await _context.Solicitacoes.Where(s => s.PetId == petId).ToListAsync();

        public async Task<IReadOnlyList<Solicitacao>> GetByTutorAsync(int tutorId)
            => await _context.Solicitacoes.Where(s => s.TutorId == tutorId).ToListAsync();

        public async Task<IReadOnlyList<Solicitacao>> GetByVeterinarioAsync(int veterinarioId)
            => await _context.Solicitacoes
                .Where(s => s.VeterinarioId == veterinarioId)
                .ToListAsync();

        public async Task<IReadOnlyList<Solicitacao>> GetByStatusAsync(string status)
            => await _context.Solicitacoes
                .Where(s => s.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

        public async Task AddAsync(Solicitacao solicitacao)
            => await _context.Solicitacoes.AddAsync(solicitacao);

        public void Remove(Solicitacao solicitacao)
            => _context.Solicitacoes.Remove(solicitacao);
    }
}
