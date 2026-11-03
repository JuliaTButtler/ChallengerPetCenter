using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.DiarioEntradas;

namespace ChallengerPetCenter.Services
{
    public interface IDiarioEntradaService
    {
        Task<IEnumerable<DiarioEntradaResponseDto>> GetAllAsync();
        Task<ServiceResult<DiarioEntradaResponseDto>> GetByIdAsync(int id);
        Task<IEnumerable<DiarioEntradaResponseDto>> GetByPetAsync(int petId);
        Task<IEnumerable<DiarioEntradaResponseDto>> GetByStatusAsync(string status);
        Task<IEnumerable<DiarioEntradaResponseDto>> GetByHumorAsync(string humor);
        Task<ServiceResult<DiarioEntradaResponseDto>> CreateAsync(CreateDiarioEntradaDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateDiarioEntradaDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
