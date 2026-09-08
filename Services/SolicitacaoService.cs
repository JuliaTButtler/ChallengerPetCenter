using ChallengerPetCenter.Common;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class SolicitacaoService : ISolicitacaoService
    {
        private readonly AppDbContext _context;

        public SolicitacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetAllAsync()
        {
            var solicitacoes = await _context.Solicitacoes.ToListAsync();
            return solicitacoes.Select(MapToDto);
        }

        public async Task<ServiceResult<SolicitacaoResponseDto>> GetByIdAsync(int id)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null)
                return ServiceResult<SolicitacaoResponseDto>.AsNotFound("Solicitação não encontrada.");

            return ServiceResult<SolicitacaoResponseDto>.Success(MapToDto(solicitacao));
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByPetAsync(int petId)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.PetId == petId)
                .ToListAsync();

            return solicitacoes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByTutorAsync(int tutorId)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.TutorId == tutorId)
                .ToListAsync();

            return solicitacoes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByVeterinarioAsync(int veterinarioId)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.VeterinarioId == veterinarioId)
                .ToListAsync();

            return solicitacoes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByStatusAsync(string status)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

            return solicitacoes.Select(MapToDto);
        }

        public async Task<ServiceResult<SolicitacaoResponseDto>> CreateAsync(CreateSolicitacaoDto dto)
        {
            var pet = await _context.Pets.FindAsync(dto.PetId);
            if (pet == null)
                return ServiceResult<SolicitacaoResponseDto>.Failure("Pet não encontrado.");

            var tutorExiste = await _context.Users.AnyAsync(u => u.Id == dto.TutorId);
            if (!tutorExiste)
                return ServiceResult<SolicitacaoResponseDto>.Failure("Tutor não encontrado.");

            if (pet.UserId != dto.TutorId)
                return ServiceResult<SolicitacaoResponseDto>.Failure(
                    "O tutor informado não é o responsável por este pet.");

            var veterinarioExiste = await _context.Veterinarios
                .AnyAsync(v => v.Id == dto.VeterinarioId);

            if (!veterinarioExiste)
                return ServiceResult<SolicitacaoResponseDto>.Failure("Veterinário não encontrado.");

            var status = string.IsNullOrWhiteSpace(dto.Status)
                ? "PENDENTE"
                : dto.Status.ToUpper();

            var solicitacao = new Solicitacao
            {
                Status = status,
                Mensagem = dto.Mensagem,
                PetId = dto.PetId,
                TutorId = dto.TutorId,
                VeterinarioId = dto.VeterinarioId,
                CriadoEm = DateTime.Now
            };

            _context.Solicitacoes.Add(solicitacao);
            await _context.SaveChangesAsync();

            return ServiceResult<SolicitacaoResponseDto>.Success(MapToDto(solicitacao));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateSolicitacaoDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _context.Solicitacoes.FindAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Solicitação não encontrada.");

            var novoStatus = dto.Status.ToUpper();
            var statusAnterior = existing.Status.ToUpper();

            existing.Status = novoStatus;
            existing.Mensagem = dto.Mensagem;

            if (dto.RespondidoEm.HasValue)
            {
                existing.RespondidoEm = dto.RespondidoEm;
            }
            else if (statusAnterior == "PENDENTE" &&
                     (novoStatus == "ACEITA" || novoStatus == "RECUSADA" || novoStatus == "ACEITO" || novoStatus == "RECUSADO"))
            {
                existing.RespondidoEm = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null)
                return ServiceResult.AsNotFound("Solicitação não encontrada.");

            _context.Solicitacoes.Remove(solicitacao);
            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        private static SolicitacaoResponseDto MapToDto(Solicitacao s) => new()
        {
            Id = s.Id,
            Status = s.Status,
            Mensagem = s.Mensagem,
            CriadoEm = s.CriadoEm,
            RespondidoEm = s.RespondidoEm,
            PetId = s.PetId,
            TutorId = s.TutorId,
            VeterinarioId = s.VeterinarioId
        };
    }
}
