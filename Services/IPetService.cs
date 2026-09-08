using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface IPetService
    {
        Task<ServiceResult<IEnumerable<PetResponseDto>>> GetAllAsync();
        Task<ServiceResult<PetResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<PetResponseDto>>> GetByUserAsync(int userId);
        Task<ServiceResult<IEnumerable<PetResponseDto>>> GetByEspecieAsync(string especie);
        Task<ServiceResult<IEnumerable<PetResponseDto>>> GetByNomeAsync(string nome);
        Task<ServiceResult<PetResponseDto>> CreateAsync(CreatePetDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
