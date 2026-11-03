using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _audit;

        public UserService(
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IAuditLogService audit)
        {
            _users = users;
            _unitOfWork = unitOfWork;
            _audit = audit;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _users.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<UserResponseDto>.AsNotFound("Usuário não encontrado.");

            return ServiceResult<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user == null)
                return ServiceResult<UserResponseDto>.AsNotFound("Usuário não encontrado.");

            return ServiceResult<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<IEnumerable<UserResponseDto>> GetByTipoAsync(string tipo)
        {
            var users = await _users.GetByTipoAsync(tipo);
            return users.Select(MapToDto);
        }

        public async Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto)
        {
            if (await _users.ExistsByEmailAsync(dto.Email))
                return ServiceResult<UserResponseDto>.Failure("Email já cadastrado.");

            var user = new User
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                Telefone = dto.Telefone,
                TipoUsuario = dto.TipoUsuario,
                Ativo = 1,
                DataCriacao = DateTime.Now
            };

            await _users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "USER_CREATED",
                "User",
                user.Id.ToString(),
                new { user.Nome, user.Email, user.TipoUsuario });

            return ServiceResult<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateUserDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existingUser = await _users.GetByIdAsync(id);
            if (existingUser == null)
                return ServiceResult.AsNotFound("Usuário não encontrado.");

            if (await _users.ExistsByEmailExcludingIdAsync(dto.Email, id))
                return ServiceResult.Failure("Email já cadastrado.");

            existingUser.Nome = dto.Nome;
            existingUser.Email = dto.Email;
            existingUser.Telefone = dto.Telefone;
            existingUser.TipoUsuario = dto.TipoUsuario;
            existingUser.Ativo = dto.Ativo;

            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "USER_UPDATED",
                "User",
                id.ToString(),
                new { dto.Nome, dto.Email, dto.TipoUsuario, dto.Ativo });

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return ServiceResult.AsNotFound("Usuário não encontrado.");

            _users.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "USER_DELETED",
                "User",
                id.ToString(),
                new { user.Email });

            return ServiceResult.Success();
        }

        private static UserResponseDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email,
            Telefone = user.Telefone,
            TipoUsuario = user.TipoUsuario,
            Ativo = user.Ativo,
            UltimoLogin = user.UltimoLogin,
            DataCriacao = user.DataCriacao
        };
    }
}
