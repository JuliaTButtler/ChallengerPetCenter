using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.Configuration;
using ChallengerPetCenter.DTOs.Auth;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Rules;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChallengerPetCenter.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwt;

        public AuthService(
            IUserRepository users,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtOptions)
        {
            _users = users;
            _unitOfWork = unitOfWork;
            _jwt = jwtOptions.Value;
        }

        public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);

            if (user == null || user.Senha != dto.Senha)
                return ServiceResult<LoginResponseDto>.Failure("Email ou senha inválidos.");

            if (!UserRules.IsActive(user.Ativo))
                return ServiceResult<LoginResponseDto>.Failure(UserRules.InactiveMessage);

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);
            var token = GenerateToken(user.Id, user.Email, user.TipoUsuario, expiresAt);

            user.UltimoLogin = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Email = user.Email,
                Nome = user.Nome,
                TipoUsuario = user.TipoUsuario
            });
        }

        private string GenerateToken(int userId, string email, string tipoUsuario, DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, tipoUsuario),
                new Claim("tipoUsuario", tipoUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
