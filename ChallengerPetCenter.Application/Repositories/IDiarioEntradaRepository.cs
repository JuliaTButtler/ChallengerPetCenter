using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IDiarioEntradaRepository
    {
        Task<IReadOnlyList<DiarioEntrada>> GetAllAsync();
        Task<DiarioEntrada?> GetByIdAsync(int id);
        Task<IReadOnlyList<DiarioEntrada>> GetByPetAsync(int petId);
        Task<IReadOnlyList<DiarioEntrada>> GetByStatusAsync(string status);
        Task<IReadOnlyList<DiarioEntrada>> GetByHumorAsync(string humor);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByPetAndDateAsync(int petId, DateTime data, int? excludeId = null);
        Task AddAsync(DiarioEntrada entrada);
        void Remove(DiarioEntrada entrada);
    }
}
