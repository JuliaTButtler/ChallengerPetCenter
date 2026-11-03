using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IPetVeterinarioRepository
    {
        Task<IReadOnlyList<PetVeterinario>> GetAllAsync();
        Task<PetVeterinario?> GetByIdAsync(int id);
        Task<IReadOnlyList<PetVeterinario>> GetByPetAsync(int petId);
        Task<IReadOnlyList<PetVeterinario>> GetByVeterinarioAsync(int veterinarioId);
        Task<IReadOnlyList<PetVeterinario>> GetAtivosAsync();
        Task<bool> ExistsActiveVinculoAsync(int petId, int veterinarioId, int? excludeId = null);
        Task AddAsync(PetVeterinario petVeterinario);
        void Remove(PetVeterinario petVeterinario);
    }
}
