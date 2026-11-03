using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Veterinarios;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class VeterinarioService : IVeterinarioService
    {
        private readonly IVeterinarioRepository _veterinarios;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;

        public VeterinarioService(
            IVeterinarioRepository veterinarios,
            IUserRepository users,
            IUnitOfWork unitOfWork)
        {
            _veterinarios = veterinarios;
            _users = users;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VeterinarioResponseDto>> GetAllAsync()
        {
            var veterinarios = await _veterinarios.GetAllAsync();
            return veterinarios.Select(MapToDto);
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByIdAsync(int id)
        {
            var veterinario = await _veterinarios.GetByIdAsync(id);
            if (veterinario == null)
                return ServiceResult<VeterinarioResponseDto>.AsNotFound("Veterinário não encontrado.");

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByCrmvAsync(string crmv)
        {
            var veterinario = await _veterinarios.GetByCrmvAsync(crmv);
            if (veterinario == null)
                return ServiceResult<VeterinarioResponseDto>.AsNotFound("Veterinário não encontrado.");

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<IEnumerable<VeterinarioResponseDto>> GetByEspecialidadeAsync(string especialidade)
        {
            var veterinarios = await _veterinarios.GetByEspecialidadeAsync(especialidade);
            return veterinarios.Select(MapToDto);
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> GetByUserIdAsync(int userId)
        {
            var veterinario = await _veterinarios.GetByUserIdAsync(userId);
            if (veterinario == null)
                return ServiceResult<VeterinarioResponseDto>.AsNotFound("Veterinário não encontrado.");

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<ServiceResult<VeterinarioResponseDto>> CreateAsync(CreateVeterinarioDto dto)
        {
            if (await _veterinarios.ExistsByCrmvAsync(dto.Crmv))
                return ServiceResult<VeterinarioResponseDto>.Failure("CRMV já cadastrado.");

            if (!await _users.ExistsAsync(dto.UserId))
                return ServiceResult<VeterinarioResponseDto>.Failure("Usuário não encontrado.");

            if (await _veterinarios.ExistsByUserIdAsync(dto.UserId))
                return ServiceResult<VeterinarioResponseDto>.Failure(
                    "Este usuário já está vinculado a um veterinário.");

            var veterinario = new Veterinario
            {
                Crmv = dto.Crmv,
                Especialidade = dto.Especialidade,
                Descricao = dto.Descricao,
                UserId = dto.UserId
            };

            await _veterinarios.AddAsync(veterinario);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<VeterinarioResponseDto>.Success(MapToDto(veterinario));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateVeterinarioDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _veterinarios.GetByIdAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Veterinário não encontrado.");

            if (await _veterinarios.ExistsByCrmvExcludingIdAsync(dto.Crmv, id))
                return ServiceResult.Failure("CRMV já cadastrado.");

            existing.Crmv = dto.Crmv;
            existing.Especialidade = dto.Especialidade;
            existing.Descricao = dto.Descricao;

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var veterinario = await _veterinarios.GetByIdAsync(id);
            if (veterinario == null)
                return ServiceResult.AsNotFound("Veterinário não encontrado.");

            _veterinarios.Remove(veterinario);
            await _unitOfWork.SaveChangesAsync();
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
