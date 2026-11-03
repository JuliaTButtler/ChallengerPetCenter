using ChallengerPetCenter.Common;
using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class UserService : IUserService
    {
        private readonly IAppDbContext _context;

        public UserService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToDto);
        }

        public async Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return ServiceResult<UserResponseDto>.AsNotFound("Usuário não encontrado.");

            return ServiceResult<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return ServiceResult<UserResponseDto>.AsNotFound("Usuário não encontrado.");

            return ServiceResult<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<IEnumerable<UserResponseDto>> GetByTipoAsync(string tipo)
        {
            tipo = tipo.ToUpper();

            var users = await _context.Users
                .Where(u => u.TipoUsuario.ToUpper() == tipo)
                .ToListAsync();

            return users.Select(MapToDto);
        }

        public async Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto)
        {
            var emailExiste = await _context.Users
                .CountAsync(u => u.Email == dto.Email) > 0;

            if (emailExiste)
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

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ServiceResult<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateUserDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return ServiceResult.AsNotFound("Usuário não encontrado.");

            var emailEmUso = await _context.Users
                .CountAsync(u => u.Email == dto.Email && u.Id != id) > 0;

            if (emailEmUso)
                return ServiceResult.Failure("Email já cadastrado.");

            existingUser.Nome = dto.Nome;
            existingUser.Email = dto.Email;
            existingUser.Telefone = dto.Telefone;
            existingUser.TipoUsuario = dto.TipoUsuario;
            existingUser.Ativo = dto.Ativo;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return ServiceResult.AsNotFound("Usuário não encontrado.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
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
