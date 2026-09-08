using ChallengerPetCenter.Common;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Veterinarios;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class VeterinarioService : IVeterinarioService
    {
        private readonly AppDbContext _context;

        public VeterinarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VeterinarioResponseDto>> GetAllAsync()
        {
            var veterinarios = await _context.Veterinarios.ToListAsync();
            return veterinarios.Select(MapToDto);
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByIdAsync(int id)
        {
            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario == null)
                return ServiceResult<VeterinarioResponseDto>.AsNotFound("Veterinário não encontrado.");

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByCrmvAsync(string crmv)
        {
            var veterinario = await _context.Veterinarios
                .FirstOrDefaultAsync(v => v.Crmv == crmv);

            if (veterinario == null)
                return ServiceResult<VeterinarioResponseDto>.AsNotFound("Veterinário não encontrado.");

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<IEnumerable<VeterinarioResponseDto>> GetByEspecialidadeAsync(string especialidade)
        {
            especialidade = especialidade.ToUpper();

            var veterinarios = await _context.Veterinarios
                .Where(v =>
                    v.Especialidade != null &&
                    v.Especialidade.ToUpper() == especialidade)
                .ToListAsync();

            return veterinarios.Select(MapToDto);
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByUserIdAsync(int userId)
        {
            var veterinario = await _context.Veterinarios
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (veterinario == null)
                return ServiceResult<VeterinarioResponseDto>.AsNotFound("Veterinário não encontrado.");

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> CreateAsync(CreateVeterinarioDto dto)
        {
            var crmvExiste = await _context.Veterinarios
                .AnyAsync(v => v.Crmv == dto.Crmv);

            if (crmvExiste)
                return ServiceResult<VeterinarioResponseDto>.Failure("CRMV já cadastrado.");

            var userExiste = await _context.Users
                .AnyAsync(u => u.Id == dto.UserId);

            if (!userExiste)
                return ServiceResult<VeterinarioResponseDto>.Failure("Usuário não encontrado.");

            var userJaVinculado = await _context.Veterinarios
                .AnyAsync(v => v.UserId == dto.UserId);

            if (userJaVinculado)
                return ServiceResult<VeterinarioResponseDto>.Failure("Este usuário já está vinculado a um veterinário.");

            var veterinario = new Veterinario
            {
                Crmv = dto.Crmv,
                Especialidade = dto.Especialidade,
                Descricao = dto.Descricao,
                UserId = dto.UserId
            };

            _context.Veterinarios.Add(veterinario);
            await _context.SaveChangesAsync();

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateVeterinarioDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _context.Veterinarios.FindAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Veterinário não encontrado.");

            var crmvEmUso = await _context.Veterinarios
                .AnyAsync(v => v.Crmv == dto.Crmv && v.Id != id);

            if (crmvEmUso)
                return ServiceResult.Failure("CRMV já cadastrado.");

            existing.Crmv = dto.Crmv;
            existing.Especialidade = dto.Especialidade;
            existing.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario == null)
                return ServiceResult.AsNotFound("Veterinário não encontrado.");

            _context.Veterinarios.Remove(veterinario);
            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        private static VeterinarioResponseDto MapToDto(Veterinario v) => new()
        {
            Id = v.Id,
            Crmv = v.Crmv,
            Especialidade = v.Especialidade,
            Descricao = v.Descricao,
            UserId = v.UserId
        };
    }
}
