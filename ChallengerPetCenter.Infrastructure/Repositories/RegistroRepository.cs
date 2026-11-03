using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class RegistroRepository : IRegistroRepository
    {
        private readonly AppDbContext _context;

        public RegistroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Registro>> GetAllAsync()
            => await _context.Registros.ToListAsync();

        public async Task<Registro?> GetByIdAsync(int id)
            => await _context.Registros.FindAsync(id);

        public async Task<IReadOnlyList<Registro>> GetByDiarioAsync(int diarioEntradaId)
            => await _context.Registros
                .Where(r => r.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

        public async Task<IReadOnlyList<Registro>> GetByTipoAsync(string tipo)
            => await _context.Registros
                .Where(r => r.Tipo.ToUpper() == tipo.ToUpper())
                .ToListAsync();

        public async Task<IReadOnlyList<Registro>> GetBySubtipoAsync(string subtipo)
            => await _context.Registros
                .Where(r =>
                    r.Subtipo != null &&
                    r.Subtipo.ToUpper() == subtipo.ToUpper())
                .ToListAsync();

        public async Task AddAsync(Registro registro)
            => await _context.Registros.AddAsync(registro);

        public void Remove(Registro registro)
            => _context.Registros.Remove(registro);
    }
}
