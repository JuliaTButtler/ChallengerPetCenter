using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class DiarioEntradaRepository : IDiarioEntradaRepository
    {
        private readonly AppDbContext _context;

        public DiarioEntradaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DiarioEntrada>> GetAllAsync()
            => await _context.DiarioEntradas.ToListAsync();

        public async Task<DiarioEntrada?> GetByIdAsync(int id)
            => await _context.DiarioEntradas.FindAsync(id);

        public async Task<IReadOnlyList<DiarioEntrada>> GetByPetAsync(int petId)
            => await _context.DiarioEntradas
                .Where(d => d.PetId == petId)
                .OrderByDescending(d => d.Data)
                .ToListAsync();

        public async Task<IReadOnlyList<DiarioEntrada>> GetByStatusAsync(string status)
            => await _context.DiarioEntradas
                .Where(d =>
                    d.Status != null &&
                    d.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

        public async Task<IReadOnlyList<DiarioEntrada>> GetByHumorAsync(string humor)
            => await _context.DiarioEntradas
                .Where(d =>
                    d.HumorGeral != null &&
                    d.HumorGeral.ToUpper() == humor.ToUpper())
                .ToListAsync();

        public async Task<bool> ExistsAsync(int id)
            => await _context.DiarioEntradas.CountAsync(d => d.Id == id) > 0;

        public async Task<bool> ExistsByPetAndDateAsync(int petId, DateTime data, int? excludeId = null)
        {
            var date = data.Date;
            return await _context.DiarioEntradas.CountAsync(d =>
                d.PetId == petId &&
                d.Data.Date == date &&
                (!excludeId.HasValue || d.Id != excludeId.Value)) > 0;
        }

        public async Task AddAsync(DiarioEntrada entrada)
            => await _context.DiarioEntradas.AddAsync(entrada);

        public void Remove(DiarioEntrada entrada)
            => _context.DiarioEntradas.Remove(entrada);
    }
}
