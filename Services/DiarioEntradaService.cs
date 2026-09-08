using ChallengerPetCenter.Data;
using ChallengerPetCenter.Domain.Rules;
using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class DiarioEntradaService : IDiarioEntradaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DiarioEntradaService> _logger;

        public DiarioEntradaService(AppDbContext context, ILogger<DiarioEntradaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetAllAsync()
        {
            var entradas = await _context.DiarioEntradas.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.Ok(entradas.Select(MapToDto));
        }

        public async Task<ServiceResult<DiarioEntradaResponseDto>> GetByIdAsync(int id)
        {
            var entrada = await _context.DiarioEntradas.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (entrada is null)
            {
                return ServiceResult<DiarioEntradaResponseDto>.NotFound("Entrada não encontrada.");
            }

            return ServiceResult<DiarioEntradaResponseDto>.Ok(MapToDto(entrada));
        }

        public async Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetByPetAsync(int petId)
        {
            var entradas = await _context.DiarioEntradas.AsNoTracking()
                .Where(d => d.PetId == petId)
                .ToListAsync();

            if (entradas.Count == 0)
            {
                return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.NotFound("Nenhuma entrada encontrada.");
            }

            return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.Ok(entradas.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetByStatusAsync(string status)
        {
            var entradas = await _context.DiarioEntradas.AsNoTracking()
                .Where(d => d.Status != null && d.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

            if (entradas.Count == 0)
            {
                return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.NotFound("Nenhuma entrada encontrada.");
            }

            return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.Ok(entradas.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<DiarioEntradaResponseDto>>> GetByHumorAsync(string humor)
        {
            var entradas = await _context.DiarioEntradas.AsNoTracking()
                .Where(d => d.HumorGeral != null && d.HumorGeral.ToUpper() == humor.ToUpper())
                .ToListAsync();

            if (entradas.Count == 0)
            {
                return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.NotFound("Nenhuma entrada encontrada.");
            }

            return ServiceResult<IEnumerable<DiarioEntradaResponseDto>>.Ok(entradas.Select(MapToDto));
        }

        public async Task<ServiceResult<DiarioEntradaResponseDto>> CreateAsync(CreateDiarioEntradaDto dto)
        {
            var petExists = await _context.Pets.AnyAsync(p => p.Id == dto.PetId);
            var jaExiste = await _context.DiarioEntradas.AnyAsync(d =>
                d.PetId == dto.PetId && d.Data.Date == dto.Data.Date);

            var validationError = DiarioEntradaRules.ValidarCriacao(dto.Data, petExists, jaExiste);
            if (validationError is not null)
            {
                return ServiceResult<DiarioEntradaResponseDto>.BadRequest(validationError);
            }

            var entrada = new DiarioEntrada
            {
                Data = dto.Data,
                Resumo = dto.Resumo,
                HumorGeral = dto.HumorGeral,
                Status = dto.Status,
                PetId = dto.PetId,
                CriadoEm = DateTime.Now
            };

            _context.DiarioEntradas.Add(entrada);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Entrada de diário {EntradaId} criada para pet {PetId}", entrada.Id, entrada.PetId);
            return ServiceResult<DiarioEntradaResponseDto>.Ok(MapToDto(entrada));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateDiarioEntradaDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existing = await _context.DiarioEntradas.FindAsync(id);
            if (existing is null)
            {
                return ServiceResult.NotFound("Entrada não encontrada.");
            }

            existing.Data = dto.Data;
            existing.Resumo = dto.Resumo;
            existing.HumorGeral = dto.HumorGeral;
            existing.Status = dto.Status;
            existing.AtualizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var entrada = await _context.DiarioEntradas.FindAsync(id);
            if (entrada is null)
            {
                return ServiceResult.NotFound("Entrada não encontrada.");
            }

            _context.DiarioEntradas.Remove(entrada);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Entrada de diário {EntradaId} removida", id);
            return ServiceResult.Ok();
        }

        private static DiarioEntradaResponseDto MapToDto(DiarioEntrada entrada) => new()
        {
            Id = entrada.Id,
            Data = entrada.Data,
            CriadoEm = entrada.CriadoEm,
            AtualizadoEm = entrada.AtualizadoEm,
            Resumo = entrada.Resumo,
            HumorGeral = entrada.HumorGeral,
            Status = entrada.Status,
            PetId = entrada.PetId
        };
    }
}
