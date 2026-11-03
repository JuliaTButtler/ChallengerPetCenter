using ChallengerPetCenter.Common;
using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IAppDbContext _context;

        public ComentarioService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetAllAsync()
        {
            var comentarios = await _context.Comentarios.ToListAsync();
            return comentarios.Select(MapToDto);
        }

        public async Task<ServiceResult<ComentarioResponseDto>> GetByIdAsync(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
                return ServiceResult<ComentarioResponseDto>.AsNotFound("Comentário não encontrado.");

            return ServiceResult<ComentarioResponseDto>.Success(MapToDto(comentario));
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetByDiarioAsync(int diarioEntradaId)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

            return comentarios.Select(MapToDto);
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetByUserAsync(int userId)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return comentarios.Select(MapToDto);
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetByTextoAsync(string texto)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.TextoComentario.ToUpper().Contains(texto.ToUpper()))
                .ToListAsync();

            return comentarios.Select(MapToDto);
        }

        public async Task<ServiceResult<ComentarioResponseDto>> CreateAsync(CreateComentarioDto dto)
        {
            var diarioExiste = await _context.DiarioEntradas
                .CountAsync(d => d.Id == dto.DiarioEntradaId) > 0;

            if (!diarioExiste)
                return ServiceResult<ComentarioResponseDto>.Failure("Entrada do diário não encontrada.");

            var userExiste = await _context.Users.CountAsync(u => u.Id == dto.UserId) > 0;
            if (!userExiste)
                return ServiceResult<ComentarioResponseDto>.Failure("Usuário não encontrado.");

            var comentario = new Comentario
            {
                TextoComentario = dto.TextoComentario,
                DiarioEntradaId = dto.DiarioEntradaId,
                UserId = dto.UserId,
                CriadoEm = DateTime.Now
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return ServiceResult<ComentarioResponseDto>.Success(MapToDto(comentario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateComentarioDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _context.Comentarios.FindAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Comentário não encontrado.");

            existing.TextoComentario = dto.TextoComentario;
            existing.EditadoEm = DateTime.Now;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
                return ServiceResult.AsNotFound("Comentário não encontrado.");

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
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
