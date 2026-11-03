using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<IReadOnlyList<User>> GetByTipoAsync(string tipo)
        {
            tipo = tipo.ToUpper();
            return await _context.Users
                .Where(u => u.TipoUsuario.ToUpper() == tipo)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Users.CountAsync(u => u.Id == id) > 0;

        public async Task<bool> ExistsByEmailAsync(string email)
            => await _context.Users.CountAsync(u => u.Email == email) > 0;

        public async Task<bool> ExistsByEmailExcludingIdAsync(string email, int excludeId)
            => await _context.Users.CountAsync(u => u.Email == email && u.Id != excludeId) > 0;

        public async Task AddAsync(User user)
            => await _context.Users.AddAsync(user);

        public void Remove(User user)
            => _context.Users.Remove(user);
    }
}
