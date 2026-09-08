using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Veterinarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class VeterinarioService : IVeterinarioService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VeterinarioService> _logger;

        public VeterinarioService(AppDbContext context, ILogger<VeterinarioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<VeterinarioResponseDto>>> GetAllAsync()
        {
            var veterinarios = await _context.Veterinarios.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<VeterinarioResponseDto>>.Ok(veterinarios.Select(MapToDto));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByIdAsync(int id)
        {
            var veterinario = await _context.Veterinarios.AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veterinario is null)
            {
                return ServiceResult<VeterinarioResponseDto>.NotFound("Veterinário não encontrado.");
            }

            return ServiceResult<VeterinarioResponseDto>.Ok(MapToDto(veterinario));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByCrmvAsync(string crmv)
        {
            var veterinario = await _context.Veterinarios.AsNoTracking()
                .FirstOrDefaultAsync(v => v.Crmv == crmv);

            if (veterinario is null)
            {
                return ServiceResult<VeterinarioResponseDto>.NotFound("Veterinário não encontrado.");
            }

            return ServiceResult<VeterinarioResponseDto>.Ok(MapToDto(veterinario));
        }

        public async Task<ServiceResult<IEnumerable<VeterinarioResponseDto>>> GetByEspecialidadeAsync(string especialidade)
        {
            especialidade = especialidade.ToUpperInvariant();

            var veterinarios = await _context.Veterinarios.AsNoTracking()
                .Where(v => v.Especialidade != null && v.Especialidade.ToUpper() == especialidade)
                .ToListAsync();

            if (veterinarios.Count == 0)
            {
                return ServiceResult<IEnumerable<VeterinarioResponseDto>>.NotFound("Nenhum veterinário encontrado.");
            }

            return ServiceResult<IEnumerable<VeterinarioResponseDto>>.Ok(veterinarios.Select(MapToDto));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByUserIdAsync(int userId)
        {
            var veterinario = await _context.Veterinarios.AsNoTracking()
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (veterinario is null)
            {
                return ServiceResult<VeterinarioResponseDto>.NotFound("Veterinário não encontrado.");
            }

            return ServiceResult<VeterinarioResponseDto>.Ok(MapToDto(veterinario));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> CreateAsync(CreateVeterinarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Crmv))
            {
                return ServiceResult<VeterinarioResponseDto>.BadRequest("CRMV é obrigatório.");
            }

            var crmvExists = await _context.Veterinarios.AnyAsync(v => v.Crmv == dto.Crmv);
            if (crmvExists)
            {
                return ServiceResult<VeterinarioResponseDto>.BadRequest("CRMV já cadastrado.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return ServiceResult<VeterinarioResponseDto>.BadRequest("Usuário não encontrado.");
            }

            var veterinario = new Veterinario
            {
                Crmv = dto.Crmv.Trim(),
                Especialidade = dto.Especialidade,
                Descricao = dto.Descricao,
                UserId = dto.UserId
            };

            _context.Veterinarios.Add(veterinario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Veterinário {VeterinarioId} criado com CRMV {Crmv}", veterinario.Id, veterinario.Crmv);
            return ServiceResult<VeterinarioResponseDto>.Ok(MapToDto(veterinario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateVeterinarioDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existing = await _context.Veterinarios.FindAsync(id);
            if (existing is null)
            {
                return ServiceResult.NotFound("Veterinário não encontrado.");
            }

            existing.Crmv = dto.Crmv;
            existing.Especialidade = dto.Especialidade;
            existing.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario is null)
            {
                return ServiceResult.NotFound("Veterinário não encontrado.");
            }

            _context.Veterinarios.Remove(veterinario);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Veterinário {VeterinarioId} removido", id);
            return ServiceResult.Ok();
        }

        private static VeterinarioResponseDto MapToDto(Veterinario veterinario) => new()
        {
            Id = veterinario.Id,
            Crmv = veterinario.Crmv,
            Especialidade = veterinario.Especialidade,
            Descricao = veterinario.Descricao,
            UserId = veterinario.UserId
        };
    }
}
