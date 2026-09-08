using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface ISolicitacaoService
    {
        Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetAllAsync();
        Task<ServiceResult<SolicitacaoResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByPetAsync(int petId);
        Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByTutorAsync(int tutorId);
        Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByVeterinarioAsync(int veterinarioId);
        Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByStatusAsync(string status);
        Task<ServiceResult<SolicitacaoResponseDto>> CreateAsync(CreateSolicitacaoDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateSolicitacaoDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
