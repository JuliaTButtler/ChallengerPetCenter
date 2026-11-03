using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly AppDbContext _context;

        public ComentarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Comentario>> GetAllAsync()
            => await _context.Comentarios.ToListAsync();

        public async Task<Comentario?> GetByIdAsync(int id)
            => await _context.Comentarios.FindAsync(id);

        public async Task<IReadOnlyList<Comentario>> GetByDiarioAsync(int diarioEntradaId)
            => await _context.Comentarios
                .Where(c => c.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

        public async Task<IReadOnlyList<Comentario>> GetByUserAsync(int userId)
            => await _context.Comentarios.Where(c => c.UserId == userId).ToListAsync();

        public async Task<IReadOnlyList<Comentario>> GetByTextoAsync(string texto)
            => await _context.Comentarios
                .Where(c => c.TextoComentario.ToUpper().Contains(texto.ToUpper()))
                .ToListAsync();

        public async Task AddAsync(Comentario comentario)
            => await _context.Comentarios.AddAsync(comentario);

        public void Remove(Comentario comentario)
            => _context.Comentarios.Remove(comentario);
    }
}
