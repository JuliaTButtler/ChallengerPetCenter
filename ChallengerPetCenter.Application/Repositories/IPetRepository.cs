using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IPetRepository
    {
        Task<IReadOnlyList<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task<IReadOnlyList<Pet>> GetByUserAsync(int userId);
        Task<IReadOnlyList<Pet>> GetByEspecieAsync(string especie);
        Task<IReadOnlyList<Pet>> GetByNomeAsync(string nome);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Pet pet);
        void Remove(Pet pet);
    }
}
