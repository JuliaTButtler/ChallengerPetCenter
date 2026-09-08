using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface IPetVeterinarioService
    {
        Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetAllAsync();
        Task<ServiceResult<PetVeterinarioResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetByPetAsync(int petId);
        Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetByVeterinarioAsync(int veterinarioId);
        Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetAtivosAsync();
        Task<ServiceResult<PetVeterinarioResponseDto>> CreateAsync(CreatePetVeterinarioDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdatePetVeterinarioDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
