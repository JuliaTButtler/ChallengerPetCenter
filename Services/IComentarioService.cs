using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface IComentarioService
    {
        Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetAllAsync();
        Task<ServiceResult<ComentarioResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetByDiarioAsync(int diarioEntradaId);
        Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetByUserAsync(int userId);
        Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetByTextoAsync(string texto);
        Task<ServiceResult<ComentarioResponseDto>> CreateAsync(CreateComentarioDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateComentarioDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
