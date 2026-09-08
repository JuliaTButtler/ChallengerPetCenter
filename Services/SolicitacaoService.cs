using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class SolicitacaoService : ISolicitacaoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SolicitacaoService> _logger;

        public SolicitacaoService(AppDbContext context, ILogger<SolicitacaoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetAllAsync()
        {
            var solicitacoes = await _context.Solicitacoes.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.Ok(solicitacoes.Select(MapToDto));
        }

        public async Task<ServiceResult<SolicitacaoResponseDto>> GetByIdAsync(int id)
        {
            var solicitacao = await _context.Solicitacoes.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitacao is null)
            {
                return ServiceResult<SolicitacaoResponseDto>.NotFound("Solicitação não encontrada.");
            }

            return ServiceResult<SolicitacaoResponseDto>.Ok(MapToDto(solicitacao));
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByPetAsync(int petId)
        {
            var solicitacoes = await _context.Solicitacoes.AsNoTracking()
                .Where(s => s.PetId == petId)
                .ToListAsync();

            if (solicitacoes.Count == 0)
            {
                return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.NotFound("Nenhuma solicitação encontrada.");
            }

            return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.Ok(solicitacoes.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByTutorAsync(int tutorId)
        {
            var solicitacoes = await _context.Solicitacoes.AsNoTracking()
                .Where(s => s.TutorId == tutorId)
                .ToListAsync();

            if (solicitacoes.Count == 0)
            {
                return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.NotFound("Nenhuma solicitação encontrada.");
            }

            return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.Ok(solicitacoes.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByVeterinarioAsync(int veterinarioId)
        {
            var solicitacoes = await _context.Solicitacoes.AsNoTracking()
                .Where(s => s.VeterinarioId == veterinarioId)
                .ToListAsync();

            if (solicitacoes.Count == 0)
            {
                return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.NotFound("Nenhuma solicitação encontrada.");
            }

            return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.Ok(solicitacoes.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoResponseDto>>> GetByStatusAsync(string status)
        {
            var solicitacoes = await _context.Solicitacoes.AsNoTracking()
                .Where(s => s.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

            if (solicitacoes.Count == 0)
            {
                return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.NotFound("Nenhuma solicitação encontrada.");
            }

            return ServiceResult<IEnumerable<SolicitacaoResponseDto>>.Ok(solicitacoes.Select(MapToDto));
        }

        public async Task<ServiceResult<SolicitacaoResponseDto>> CreateAsync(CreateSolicitacaoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return ServiceResult<SolicitacaoResponseDto>.BadRequest("Status é obrigatório.");
            }

            if (!await _context.Pets.AnyAsync(p => p.Id == dto.PetId))
            {
                return ServiceResult<SolicitacaoResponseDto>.BadRequest("Pet não encontrado.");
            }

            if (!await _context.Users.AnyAsync(u => u.Id == dto.TutorId))
            {
                return ServiceResult<SolicitacaoResponseDto>.BadRequest("Tutor não encontrado.");
            }

            if (!await _context.Veterinarios.AnyAsync(v => v.Id == dto.VeterinarioId))
            {
                return ServiceResult<SolicitacaoResponseDto>.BadRequest("Veterinário não encontrado.");
            }

            var solicitacao = new Solicitacao
            {
                Status = dto.Status.Trim().ToUpperInvariant(),
                Mensagem = dto.Mensagem,
                PetId = dto.PetId,
                TutorId = dto.TutorId,
                VeterinarioId = dto.VeterinarioId,
                CriadoEm = DateTime.Now
            };

            _context.Solicitacoes.Add(solicitacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitação {SolicitacaoId} criada para pet {PetId}", solicitacao.Id, solicitacao.PetId);
            return ServiceResult<SolicitacaoResponseDto>.Ok(MapToDto(solicitacao));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateSolicitacaoDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existing = await _context.Solicitacoes.FindAsync(id);
            if (existing is null)
            {
                return ServiceResult.NotFound("Solicitação não encontrada.");
            }

            existing.Status = dto.Status;
            existing.Mensagem = dto.Mensagem;
            existing.RespondidoEm = dto.RespondidoEm;

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao is null)
            {
                return ServiceResult.NotFound("Solicitação não encontrada.");
            }

            _context.Solicitacoes.Remove(solicitacao);
            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        private static SolicitacaoResponseDto MapToDto(Solicitacao solicitacao) => new()
        {
            Id = solicitacao.Id,
            Status = solicitacao.Status,
            Mensagem = solicitacao.Mensagem,
            CriadoEm = solicitacao.CriadoEm,
            RespondidoEm = solicitacao.RespondidoEm,
            PetId = solicitacao.PetId,
            TutorId = solicitacao.TutorId,
            VeterinarioId = solicitacao.VeterinarioId
        };
    }
}
