using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class PetVeterinarioRepository : IPetVeterinarioRepository
    {
        private readonly AppDbContext _context;

        public PetVeterinarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PetVeterinario>> GetAllAsync()
            => await _context.PetVeterinarios.ToListAsync();

        public async Task<PetVeterinario?> GetByIdAsync(int id)
            => await _context.PetVeterinarios.FindAsync(id);

        public async Task<IReadOnlyList<PetVeterinario>> GetByPetAsync(int petId)
            => await _context.PetVeterinarios.Where(pv => pv.PetId == petId).ToListAsync();

        public async Task<IReadOnlyList<PetVeterinario>> GetByVeterinarioAsync(int veterinarioId)
            => await _context.PetVeterinarios
                .Where(pv => pv.VeterinarioId == veterinarioId)
                .ToListAsync();

        public async Task<IReadOnlyList<PetVeterinario>> GetAtivosAsync()
            => await _context.PetVeterinarios.Where(pv => pv.Ativo == 1).ToListAsync();

        public async Task<bool> ExistsActiveVinculoAsync(int petId, int veterinarioId, int? excludeId = null)
            => await _context.PetVeterinarios.CountAsync(pv =>
                pv.PetId == petId &&
                pv.VeterinarioId == veterinarioId &&
                pv.Ativo == 1 &&
                (!excludeId.HasValue || pv.Id != excludeId.Value)) > 0;

        public async Task AddAsync(PetVeterinario petVeterinario)
            => await _context.PetVeterinarios.AddAsync(petVeterinario);

        public void Remove(PetVeterinario petVeterinario)
            => _context.PetVeterinarios.Remove(petVeterinario);
    }
}
