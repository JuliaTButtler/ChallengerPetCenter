using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ComentarioService> _logger;

        public ComentarioService(AppDbContext context, ILogger<ComentarioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetAllAsync()
        {
            var comentarios = await _context.Comentarios.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<ComentarioResponseDto>>.Ok(comentarios.Select(MapToDto));
        }

        public async Task<ServiceResult<ComentarioResponseDto>> GetByIdAsync(int id)
        {
            var comentario = await _context.Comentarios.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comentario is null)
            {
                return ServiceResult<ComentarioResponseDto>.NotFound("Comentário não encontrado.");
            }

            return ServiceResult<ComentarioResponseDto>.Ok(MapToDto(comentario));
        }

        public async Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetByDiarioAsync(int diarioEntradaId)
        {
            var comentarios = await _context.Comentarios.AsNoTracking()
                .Where(c => c.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

            if (comentarios.Count == 0)
            {
                return ServiceResult<IEnumerable<ComentarioResponseDto>>.NotFound("Nenhum comentário encontrado.");
            }

            return ServiceResult<IEnumerable<ComentarioResponseDto>>.Ok(comentarios.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetByUserAsync(int userId)
        {
            var comentarios = await _context.Comentarios.AsNoTracking()
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (comentarios.Count == 0)
            {
                return ServiceResult<IEnumerable<ComentarioResponseDto>>.NotFound("Nenhum comentário encontrado.");
            }

            return ServiceResult<IEnumerable<ComentarioResponseDto>>.Ok(comentarios.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<ComentarioResponseDto>>> GetByTextoAsync(string texto)
        {
            var comentarios = await _context.Comentarios.AsNoTracking()
                .Where(c => c.TextoComentario.ToUpper().Contains(texto.ToUpper()))
                .ToListAsync();

            if (comentarios.Count == 0)
            {
                return ServiceResult<IEnumerable<ComentarioResponseDto>>.NotFound("Nenhum comentário encontrado.");
            }

            return ServiceResult<IEnumerable<ComentarioResponseDto>>.Ok(comentarios.Select(MapToDto));
        }

        public async Task<ServiceResult<ComentarioResponseDto>> CreateAsync(CreateComentarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TextoComentario))
            {
                return ServiceResult<ComentarioResponseDto>.BadRequest("Texto do comentário é obrigatório.");
            }

            var diarioExists = await _context.DiarioEntradas.AnyAsync(d => d.Id == dto.DiarioEntradaId);
            if (!diarioExists)
            {
                return ServiceResult<ComentarioResponseDto>.BadRequest("Entrada do diário não encontrada.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return ServiceResult<ComentarioResponseDto>.BadRequest("Usuário não encontrado.");
            }

            var comentario = new Comentario
            {
                TextoComentario = dto.TextoComentario.Trim(),
                DiarioEntradaId = dto.DiarioEntradaId,
                UserId = dto.UserId,
                CriadoEm = DateTime.Now
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comentário {ComentarioId} criado pelo usuário {UserId}", comentario.Id, comentario.UserId);
            return ServiceResult<ComentarioResponseDto>.Ok(MapToDto(comentario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateComentarioDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existing = await _context.Comentarios.FindAsync(id);
            if (existing is null)
            {
                return ServiceResult.NotFound("Comentário não encontrado.");
            }

            existing.TextoComentario = dto.TextoComentario;
            existing.EditadoEm = DateTime.Now;

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario is null)
            {
                return ServiceResult.NotFound("Comentário não encontrado.");
            }

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        private static ComentarioResponseDto MapToDto(Comentario comentario) => new()
        {
            Id = comentario.Id,
            TextoComentario = comentario.TextoComentario,
            CriadoEm = comentario.CriadoEm,
            EditadoEm = comentario.EditadoEm,
            DiarioEntradaId = comentario.DiarioEntradaId,
            UserId = comentario.UserId
        };
    }
}
