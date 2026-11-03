using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarios;
        private readonly IDiarioEntradaRepository _diarioEntradas;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _audit;

        public ComentarioService(
            IComentarioRepository comentarios,
            IDiarioEntradaRepository diarioEntradas,
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IAuditLogService audit)
        {
            _comentarios = comentarios;
            _diarioEntradas = diarioEntradas;
            _users = users;
            _unitOfWork = unitOfWork;
            _audit = audit;
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetAllAsync()
        {
            var comentarios = await _comentarios.GetAllAsync();
            return comentarios.Select(MapToDto);
        }

        public async Task<ServiceResult<ComentarioResponseDto>> GetByIdAsync(int id)
        {
            var comentario = await _comentarios.GetByIdAsync(id);
            if (comentario == null)
                return ServiceResult<ComentarioResponseDto>.AsNotFound("Comentário não encontrado.");

            return ServiceResult<ComentarioResponseDto>.Success(MapToDto(comentario));
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetByDiarioAsync(int diarioEntradaId)
        {
            var comentarios = await _comentarios.GetByDiarioAsync(diarioEntradaId);
            return comentarios.Select(MapToDto);
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetByUserAsync(int userId)
        {
            var comentarios = await _comentarios.GetByUserAsync(userId);
            return comentarios.Select(MapToDto);
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetByTextoAsync(string texto)
        {
            var comentarios = await _comentarios.GetByTextoAsync(texto);
            return comentarios.Select(MapToDto);
        }

        public async Task<ServiceResult<ComentarioResponseDto>> CreateAsync(CreateComentarioDto dto)
        {
            if (!await _diarioEntradas.ExistsAsync(dto.DiarioEntradaId))
                return ServiceResult<ComentarioResponseDto>.Failure("Entrada do diário não encontrada.");

            if (!await _users.ExistsAsync(dto.UserId))
                return ServiceResult<ComentarioResponseDto>.Failure("Usuário não encontrado.");

            var comentario = new Comentario
            {
                TextoComentario = dto.TextoComentario,
                DiarioEntradaId = dto.DiarioEntradaId,
                UserId = dto.UserId,
                CriadoEm = DateTime.Now
            };

            await _comentarios.AddAsync(comentario);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "COMENTARIO_CREATED",
                "Comentario",
                comentario.Id.ToString(),
                new { comentario.DiarioEntradaId, comentario.UserId });

            return ServiceResult<ComentarioResponseDto>.Success(MapToDto(comentario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateComentarioDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _comentarios.GetByIdAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Comentário não encontrado.");

            existing.TextoComentario = dto.TextoComentario;
            existing.EditadoEm = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "COMENTARIO_UPDATED",
                "Comentario",
                id.ToString());
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var comentario = await _comentarios.GetByIdAsync(id);
            if (comentario == null)
                return ServiceResult.AsNotFound("Comentário não encontrado.");

            _comentarios.Remove(comentario);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "COMENTARIO_DELETED",
                "Comentario",
                id.ToString());
            return ServiceResult.Success();
        }

        private static ComentarioResponseDto MapToDto(Comentario c) => new()
        {
            Id = c.Id,
            TextoComentario = c.TextoComentario,
            CriadoEm = c.CriadoEm,
            EditadoEm = c.EditadoEm,
            DiarioEntradaId = c.DiarioEntradaId,
            UserId = c.UserId
        };
    }
}
