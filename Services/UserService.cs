using ChallengerPetCenter.Data;
using ChallengerPetCenter.Domain.Rules;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<UserResponseDto>>> GetAllAsync()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<UserResponseDto>>.Ok(users.Select(MapToDto));
        }

        public async Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                return ServiceResult<UserResponseDto>.NotFound("Usuário não encontrado.");
            }

            return ServiceResult<UserResponseDto>.Ok(MapToDto(user));
        }

        public async Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                return ServiceResult<UserResponseDto>.NotFound("Usuário não encontrado.");
            }

            return ServiceResult<UserResponseDto>.Ok(MapToDto(user));
        }

        public async Task<ServiceResult<IEnumerable<UserResponseDto>>> GetByTipoAsync(string tipo)
        {
            tipo = tipo.ToUpperInvariant();

            var users = await _context.Users.AsNoTracking()
                .Where(u => u.TipoUsuario.ToUpper() == tipo)
                .ToListAsync();

            if (users.Count == 0)
            {
                return ServiceResult<IEnumerable<UserResponseDto>>.NotFound("Nenhum usuário encontrado.");
            }

            return ServiceResult<IEnumerable<UserResponseDto>>.Ok(users.Select(MapToDto));
        }

        public async Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto)
        {
            var validationError = UserRules.ValidarCriacao(dto.Nome, dto.Email, dto.TipoUsuario);
            if (validationError is not null)
            {
                return ServiceResult<UserResponseDto>.BadRequest(validationError);
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                return ServiceResult<UserResponseDto>.BadRequest("Email já cadastrado.");
            }

            var user = new User
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                Senha = dto.Senha,
                Telefone = dto.Telefone,
                TipoUsuario = dto.TipoUsuario.Trim().ToUpperInvariant(),
                Ativo = dto.Ativo,
                DataCriacao = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário {UserId} criado com email {Email}", user.Id, user.Email);
            return ServiceResult<UserResponseDto>.Ok(MapToDto(user));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateUserDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser is null)
            {
                return ServiceResult.NotFound("Usuário não encontrado.");
            }

            existingUser.Nome = dto.Nome;
            existingUser.Email = dto.Email;
            existingUser.Telefone = dto.Telefone;
            existingUser.TipoUsuario = dto.TipoUsuario;
            existingUser.Ativo = dto.Ativo;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário {UserId} atualizado", id);
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return ServiceResult.NotFound("Usuário não encontrado.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Usuário {UserId} removido", id);
            return ServiceResult.Ok();
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
