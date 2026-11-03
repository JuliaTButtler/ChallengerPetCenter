using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IVeterinarioRepository
    {
        Task<IReadOnlyList<Veterinario>> GetAllAsync();
        Task<Veterinario?> GetByIdAsync(int id);
        Task<Veterinario?> GetByCrmvAsync(string crmv);
        Task<IReadOnlyList<Veterinario>> GetByEspecialidadeAsync(string especialidade);
        Task<Veterinario?> GetByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCrmvAsync(string crmv);
        Task<bool> ExistsByCrmvExcludingIdAsync(string crmv, int excludeId);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task AddAsync(Veterinario veterinario);
        void Remove(Veterinario veterinario);
    }
}
