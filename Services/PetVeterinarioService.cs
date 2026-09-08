using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class PetVeterinarioService : IPetVeterinarioService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PetVeterinarioService> _logger;

        public PetVeterinarioService(AppDbContext context, ILogger<PetVeterinarioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetAllAsync()
        {
            var vinculos = await _context.PetVeterinarios.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.Ok(vinculos.Select(MapToDto));
        }

        public async Task<ServiceResult<PetVeterinarioResponseDto>> GetByIdAsync(int id)
        {
            var vinculo = await _context.PetVeterinarios.AsNoTracking()
                .FirstOrDefaultAsync(pv => pv.Id == id);

            if (vinculo is null)
            {
                return ServiceResult<PetVeterinarioResponseDto>.NotFound("Vínculo não encontrado.");
            }

            return ServiceResult<PetVeterinarioResponseDto>.Ok(MapToDto(vinculo));
        }

        public async Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetByPetAsync(int petId)
        {
            var vinculos = await _context.PetVeterinarios.AsNoTracking()
                .Where(pv => pv.PetId == petId)
                .ToListAsync();

            if (vinculos.Count == 0)
            {
                return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.NotFound("Nenhum vínculo encontrado.");
            }

            return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.Ok(vinculos.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetByVeterinarioAsync(int veterinarioId)
        {
            var vinculos = await _context.PetVeterinarios.AsNoTracking()
                .Where(pv => pv.VeterinarioId == veterinarioId)
                .ToListAsync();

            if (vinculos.Count == 0)
            {
                return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.NotFound("Nenhum vínculo encontrado.");
            }

            return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.Ok(vinculos.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<PetVeterinarioResponseDto>>> GetAtivosAsync()
        {
            var vinculos = await _context.PetVeterinarios.AsNoTracking()
                .Where(pv => pv.Ativo == 1)
                .ToListAsync();

            if (vinculos.Count == 0)
            {
                return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.NotFound("Nenhum vínculo ativo encontrado.");
            }

            return ServiceResult<IEnumerable<PetVeterinarioResponseDto>>.Ok(vinculos.Select(MapToDto));
        }

        public async Task<ServiceResult<PetVeterinarioResponseDto>> CreateAsync(CreatePetVeterinarioDto dto)
        {
            if (!await _context.Pets.AnyAsync(p => p.Id == dto.PetId))
            {
                return ServiceResult<PetVeterinarioResponseDto>.BadRequest("Pet não encontrado.");
            }

            if (!await _context.Veterinarios.AnyAsync(v => v.Id == dto.VeterinarioId))
            {
                return ServiceResult<PetVeterinarioResponseDto>.BadRequest("Veterinário não encontrado.");
            }

            var vinculoAtivoExiste = await _context.PetVeterinarios.AnyAsync(pv =>
                pv.PetId == dto.PetId &&
                pv.VeterinarioId == dto.VeterinarioId &&
                pv.Ativo == 1);

            if (vinculoAtivoExiste)
            {
                return ServiceResult<PetVeterinarioResponseDto>.BadRequest(
                    "Já existe um vínculo ativo entre este pet e veterinário.");
            }

            var vinculo = new PetVeterinario
            {
                DataInicio = dto.DataInicio ?? DateTime.Now,
                Ativo = dto.Ativo,
                Observacoes = dto.Observacoes,
                PetId = dto.PetId,
                VeterinarioId = dto.VeterinarioId
            };

            _context.PetVeterinarios.Add(vinculo);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Vínculo {VinculoId} criado entre pet {PetId} e veterinário {VeterinarioId}",
                vinculo.Id, vinculo.PetId, vinculo.VeterinarioId);

            return ServiceResult<PetVeterinarioResponseDto>.Ok(MapToDto(vinculo));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetVeterinarioDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existing = await _context.PetVeterinarios.FindAsync(id);
            if (existing is null)
            {
                return ServiceResult.NotFound("Vínculo não encontrado.");
            }

            existing.Ativo = dto.Ativo;
            existing.Observacoes = dto.Observacoes;

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var vinculo = await _context.PetVeterinarios.FindAsync(id);
            if (vinculo is null)
            {
                return ServiceResult.NotFound("Vínculo não encontrado.");
            }

            _context.PetVeterinarios.Remove(vinculo);
            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        private static PetVeterinarioResponseDto MapToDto(PetVeterinario vinculo) => new()
        {
            Id = vinculo.Id,
            DataInicio = vinculo.DataInicio,
            Ativo = vinculo.Ativo,
            Observacoes = vinculo.Observacoes,
            PetId = vinculo.PetId,
            VeterinarioId = vinculo.VeterinarioId
        };
    }
}
