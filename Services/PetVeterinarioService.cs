using ChallengerPetCenter.Common;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class PetVeterinarioService : IPetVeterinarioService
    {
        private readonly AppDbContext _context;

        public PetVeterinarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetAllAsync()
        {
            var items = await _context.PetVeterinarios.ToListAsync();
            return items.Select(MapToDto);
        }

        public async Task<ServiceResult<PetVeterinarioResponseDto>> GetByIdAsync(int id)
        {
            var item = await _context.PetVeterinarios.FindAsync(id);
            if (item == null)
                return ServiceResult<PetVeterinarioResponseDto>.AsNotFound("Vínculo não encontrado.");

            return ServiceResult<PetVeterinarioResponseDto>.Success(MapToDto(item));
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetByPetAsync(int petId)
        {
            var items = await _context.PetVeterinarios
                .Where(pv => pv.PetId == petId)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetByVeterinarioAsync(int veterinarioId)
        {
            var items = await _context.PetVeterinarios
                .Where(pv => pv.VeterinarioId == veterinarioId)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetAtivosAsync()
        {
            var items = await _context.PetVeterinarios
                .Where(pv => pv.Ativo == 1)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<ServiceResult<PetVeterinarioResponseDto>> CreateAsync(CreatePetVeterinarioDto dto)
        {
            var petExiste = await _context.Pets.AnyAsync(p => p.Id == dto.PetId);
            if (!petExiste)
                return ServiceResult<PetVeterinarioResponseDto>.Failure("Pet não encontrado.");

            var veterinarioExiste = await _context.Veterinarios
                .AnyAsync(v => v.Id == dto.VeterinarioId);

            if (!veterinarioExiste)
                return ServiceResult<PetVeterinarioResponseDto>.Failure("Veterinário não encontrado.");

            var vinculoAtivoExiste = await _context.PetVeterinarios
                .AnyAsync(pv =>
                    pv.PetId == dto.PetId &&
                    pv.VeterinarioId == dto.VeterinarioId &&
                    pv.Ativo == 1);

            if (vinculoAtivoExiste)
                return ServiceResult<PetVeterinarioResponseDto>.Failure(
                    "Já existe um vínculo ativo entre este pet e veterinário.");

            var petVeterinario = new PetVeterinario
            {
                PetId = dto.PetId,
                VeterinarioId = dto.VeterinarioId,
                DataInicio = dto.DataInicio ?? DateTime.Now,
                Ativo = 1,
                Observacoes = dto.Observacoes
            };

            _context.PetVeterinarios.Add(petVeterinario);
            await _context.SaveChangesAsync();

            return ServiceResult<PetVeterinarioResponseDto>.Success(MapToDto(petVeterinario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetVeterinarioDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _context.PetVeterinarios.FindAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Vínculo não encontrado.");

            // Ao reativar, garantir que não haja outro vínculo ativo do mesmo par
            if (dto.Ativo == 1 && existing.Ativo != 1)
            {
                var outroAtivo = await _context.PetVeterinarios
                    .AnyAsync(pv =>
                        pv.Id != id &&
                        pv.PetId == existing.PetId &&
                        pv.VeterinarioId == existing.VeterinarioId &&
                        pv.Ativo == 1);

                if (outroAtivo)
                    return ServiceResult.Failure(
                        "Já existe um vínculo ativo entre este pet e veterinário.");
            }

            existing.Ativo = dto.Ativo;
            existing.Observacoes = dto.Observacoes;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var item = await _context.PetVeterinarios.FindAsync(id);
            if (item == null)
                return ServiceResult.AsNotFound("Vínculo não encontrado.");

            _context.PetVeterinarios.Remove(item);
            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        private static PetVeterinarioResponseDto MapToDto(PetVeterinario pv) => new()
        {
            Id = pv.Id,
            DataInicio = pv.DataInicio,
            Ativo = pv.Ativo,
            Observacoes = pv.Observacoes,
            PetId = pv.PetId,
            VeterinarioId = pv.VeterinarioId
        };
    }
}
