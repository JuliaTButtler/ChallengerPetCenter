using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Veterinarios;

namespace ChallengerPetCenter.Services
{
    public interface IVeterinarioService
    {
        Task<IEnumerable<VeterinarioResponseDto>> GetAllAsync();
        Task<ServiceResult<VeterinarioResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<VeterinarioResponseDto>> GetByCrmvAsync(string crmv);
        Task<IEnumerable<VeterinarioResponseDto>> GetByEspecialidadeAsync(string especialidade);
        Task<ServiceResult<VeterinarioResponseDto>> GetByUserIdAsync(int userId);
        Task<ServiceResult<VeterinarioResponseDto>> CreateAsync(CreateVeterinarioDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateVeterinarioDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
