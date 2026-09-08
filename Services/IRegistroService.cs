using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Registros;

namespace ChallengerPetCenter.Services
{
    public interface IRegistroService
    {
        Task<IEnumerable<RegistroResponseDto>> GetAllAsync();
        Task<ServiceResult<RegistroResponseDto>> GetByIdAsync(int id);
        Task<IEnumerable<RegistroResponseDto>> GetByDiarioAsync(int diarioEntradaId);
        Task<IEnumerable<RegistroResponseDto>> GetByTipoAsync(string tipo);
        Task<IEnumerable<RegistroResponseDto>> GetBySubtipoAsync(string subtipo);
        Task<ServiceResult<RegistroResponseDto>> CreateAsync(CreateRegistroDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateRegistroDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
