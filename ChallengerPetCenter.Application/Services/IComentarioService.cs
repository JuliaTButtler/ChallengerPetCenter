using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Comentarios;

namespace ChallengerPetCenter.Services
{
    public interface IComentarioService
    {
        Task<IEnumerable<ComentarioResponseDto>> GetAllAsync();
        Task<ServiceResult<ComentarioResponseDto>> GetByIdAsync(int id);
        Task<IEnumerable<ComentarioResponseDto>> GetByDiarioAsync(int diarioEntradaId);
        Task<IEnumerable<ComentarioResponseDto>> GetByUserAsync(int userId);
        Task<IEnumerable<ComentarioResponseDto>> GetByTextoAsync(string texto);
        Task<ServiceResult<ComentarioResponseDto>> CreateAsync(CreateComentarioDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateComentarioDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
