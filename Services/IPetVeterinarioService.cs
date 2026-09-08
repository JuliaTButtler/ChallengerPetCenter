using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.PetVeterinarios;

namespace ChallengerPetCenter.Services
{
    public interface IPetVeterinarioService
    {
        Task<IEnumerable<PetVeterinarioResponseDto>> GetAllAsync();
        Task<ServiceResult<PetVeterinarioResponseDto>> GetByIdAsync(int id);
        Task<IEnumerable<PetVeterinarioResponseDto>> GetByPetAsync(int petId);
        Task<IEnumerable<PetVeterinarioResponseDto>> GetByVeterinarioAsync(int veterinarioId);
        Task<IEnumerable<PetVeterinarioResponseDto>> GetAtivosAsync();
        Task<ServiceResult<PetVeterinarioResponseDto>> CreateAsync(CreatePetVeterinarioDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdatePetVeterinarioDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
