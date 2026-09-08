using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface IDiarioEntradaService
    {
        Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetAllAsync();
        Task<ServiceResult<DiarioEntradaResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetByPetAsync(int petId);
        Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetByStatusAsync(string status);
        Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetByHumorAsync(string humor);
        Task<ServiceResult<DiarioEntradaResponseDto>> CreateAsync(CreateDiarioEntradaDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateDiarioEntradaDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
