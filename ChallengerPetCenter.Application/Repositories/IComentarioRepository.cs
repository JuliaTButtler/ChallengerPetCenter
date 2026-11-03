using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IComentarioRepository
    {
        Task<IReadOnlyList<Comentario>> GetAllAsync();
        Task<Comentario?> GetByIdAsync(int id);
        Task<IReadOnlyList<Comentario>> GetByDiarioAsync(int diarioEntradaId);
        Task<IReadOnlyList<Comentario>> GetByUserAsync(int userId);
        Task<IReadOnlyList<Comentario>> GetByTextoAsync(string texto);
        Task AddAsync(Comentario comentario);
        void Remove(Comentario comentario);
    }
}
