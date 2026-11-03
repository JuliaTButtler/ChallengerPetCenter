using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Pet>> GetAllAsync()
            => await _context.Pets.ToListAsync();

        public async Task<Pet?> GetByIdAsync(int id)
            => await _context.Pets.FindAsync(id);

        public async Task<IReadOnlyList<Pet>> GetByUserAsync(int userId)
            => await _context.Pets.Where(p => p.UserId == userId).ToListAsync();

        public async Task<IReadOnlyList<Pet>> GetByEspecieAsync(string especie)
            => await _context.Pets
                .Where(p => p.Especie.ToUpper() == especie.ToUpper())
                .ToListAsync();

        public async Task<IReadOnlyList<Pet>> GetByNomeAsync(string nome)
            => await _context.Pets
                .Where(p => p.Nome.ToUpper().Contains(nome.ToUpper()))
                .ToListAsync();

        public async Task<bool> ExistsAsync(int id)
            => await _context.Pets.CountAsync(p => p.Id == id) > 0;

        public async Task AddAsync(Pet pet)
            => await _context.Pets.AddAsync(pet);

        public void Remove(Pet pet)
            => _context.Pets.Remove(pet);
    }
}
