using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class PetVeterinarioService : IPetVeterinarioService
    {
        private readonly IPetVeterinarioRepository _petVeterinarios;
        private readonly IPetRepository _pets;
        private readonly IVeterinarioRepository _veterinarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _audit;

        public PetVeterinarioService(
            IPetVeterinarioRepository petVeterinarios,
            IPetRepository pets,
            IVeterinarioRepository veterinarios,
            IUnitOfWork unitOfWork,
            IAuditLogService audit)
        {
            _petVeterinarios = petVeterinarios;
            _pets = pets;
            _veterinarios = veterinarios;
            _unitOfWork = unitOfWork;
            _audit = audit;
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetAllAsync()
        {
            var items = await _petVeterinarios.GetAllAsync();
            return items.Select(MapToDto);
        }

        public async Task<ServiceResult<PetVeterinarioResponseDto>> GetByIdAsync(int id)
        {
            var item = await _petVeterinarios.GetByIdAsync(id);
            if (item == null)
                return ServiceResult<PetVeterinarioResponseDto>.AsNotFound("Vínculo não encontrado.");

            return ServiceResult<PetVeterinarioResponseDto>.Success(MapToDto(item));
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetByPetAsync(int petId)
        {
            var items = await _petVeterinarios.GetByPetAsync(petId);
            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetByVeterinarioAsync(int veterinarioId)
        {
            var items = await _petVeterinarios.GetByVeterinarioAsync(veterinarioId);
            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<PetVeterinarioResponseDto>> GetAtivosAsync()
        {
            var items = await _petVeterinarios.GetAtivosAsync();
            return items.Select(MapToDto);
        }

        public async Task<ServiceResult<PetVeterinarioResponseDto>> CreateAsync(CreatePetVeterinarioDto dto)
        {
            if (!await _pets.ExistsAsync(dto.PetId))
                return ServiceResult<PetVeterinarioResponseDto>.Failure("Pet não encontrado.");

            if (!await _veterinarios.ExistsAsync(dto.VeterinarioId))
                return ServiceResult<PetVeterinarioResponseDto>.Failure("Veterinário não encontrado.");

            if (await _petVeterinarios.ExistsActiveVinculoAsync(dto.PetId, dto.VeterinarioId))
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

            await _petVeterinarios.AddAsync(petVeterinario);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "PET_VETERINARIO_CREATED",
                "PetVeterinario",
                petVeterinario.Id.ToString(),
                new { petVeterinario.PetId, petVeterinario.VeterinarioId });

            return ServiceResult<PetVeterinarioResponseDto>.Success(MapToDto(petVeterinario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetVeterinarioDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _petVeterinarios.GetByIdAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Vínculo não encontrado.");

            if (dto.Ativo == 1 && existing.Ativo != 1)
            {
                if (await _petVeterinarios.ExistsActiveVinculoAsync(
                        existing.PetId, existing.VeterinarioId, excludeId: id))
                {
                    return ServiceResult.Failure(
                        "Já existe um vínculo ativo entre este pet e veterinário.");
                }
            }

            existing.Ativo = dto.Ativo;
            existing.Observacoes = dto.Observacoes;

            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "PET_VETERINARIO_UPDATED",
                "PetVeterinario",
                id.ToString(),
                new { dto.Ativo });
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var item = await _petVeterinarios.GetByIdAsync(id);
            if (item == null)
                return ServiceResult.AsNotFound("Vínculo não encontrado.");

            _petVeterinarios.Remove(item);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "PET_VETERINARIO_DELETED",
                "PetVeterinario",
                id.ToString(),
                new { item.PetId, item.VeterinarioId });
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
