using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Solicitacoes;

namespace ChallengerPetCenter.Services
{
    public interface ISolicitacaoService
    {
        Task<IEnumerable<SolicitacaoResponseDto>> GetAllAsync();
        Task<ServiceResult<SolicitacaoResponseDto>> GetByIdAsync(int id);
        Task<IEnumerable<SolicitacaoResponseDto>> GetByPetAsync(int petId);
        Task<IEnumerable<SolicitacaoResponseDto>> GetByTutorAsync(int tutorId);
        Task<IEnumerable<SolicitacaoResponseDto>> GetByVeterinarioAsync(int veterinarioId);
        Task<IEnumerable<SolicitacaoResponseDto>> GetByStatusAsync(string status);
        Task<ServiceResult<SolicitacaoResponseDto>> CreateAsync(CreateSolicitacaoDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateSolicitacaoDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
