using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Pets;

namespace ChallengerPetCenter.Services
{
    public interface IPetService
    {
        Task<IEnumerable<PetResponseDto>> GetAllAsync();
        Task<ServiceResult<PetResponseDto>> GetByIdAsync(int id);
        Task<IEnumerable<PetResponseDto>> GetByUserAsync(int userId);
        Task<IEnumerable<PetResponseDto>> GetByEspecieAsync(string especie);
        Task<IEnumerable<PetResponseDto>> GetByNomeAsync(string nome);
        Task<ServiceResult<PetResponseDto>> CreateAsync(CreatePetDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
