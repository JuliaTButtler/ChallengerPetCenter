using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class VeterinarioRepository : IVeterinarioRepository
    {
        private readonly AppDbContext _context;

        public VeterinarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Veterinario>> GetAllAsync()
            => await _context.Veterinarios.ToListAsync();

        public async Task<Veterinario?> GetByIdAsync(int id)
            => await _context.Veterinarios.FindAsync(id);

        public async Task<Veterinario?> GetByCrmvAsync(string crmv)
            => await _context.Veterinarios.FirstOrDefaultAsync(v => v.Crmv == crmv);

        public async Task<IReadOnlyList<Veterinario>> GetByEspecialidadeAsync(string especialidade)
        {
            especialidade = especialidade.ToUpper();
            return await _context.Veterinarios
                .Where(v =>
                    v.Especialidade != null &&
                    v.Especialidade.ToUpper() == especialidade)
                .ToListAsync();
        }

        public async Task<Veterinario?> GetByUserIdAsync(int userId)
            => await _context.Veterinarios.FirstOrDefaultAsync(v => v.UserId == userId);

        public async Task<bool> ExistsAsync(int id)
            => await _context.Veterinarios.CountAsync(v => v.Id == id) > 0;

        public async Task<bool> ExistsByCrmvAsync(string crmv)
            => await _context.Veterinarios.CountAsync(v => v.Crmv == crmv) > 0;

        public async Task<bool> ExistsByCrmvExcludingIdAsync(string crmv, int excludeId)
            => await _context.Veterinarios.CountAsync(v => v.Crmv == crmv && v.Id != excludeId) > 0;

        public async Task<bool> ExistsByUserIdAsync(int userId)
            => await _context.Veterinarios.CountAsync(v => v.UserId == userId) > 0;

        public async Task AddAsync(Veterinario veterinario)
            => await _context.Veterinarios.AddAsync(veterinario);

        public void Remove(Veterinario veterinario)
            => _context.Veterinarios.Remove(veterinario);
    }
}
