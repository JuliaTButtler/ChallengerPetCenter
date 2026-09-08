using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface IRegistroService
    {
        Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetAllAsync();
        Task<ServiceResult<RegistroResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetByDiarioAsync(int diarioEntradaId);
        Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetByTipoAsync(string tipo);
        Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetBySubtipoAsync(string subtipo);
        Task<ServiceResult<RegistroResponseDto>> CreateAsync(CreateRegistroDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateRegistroDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
