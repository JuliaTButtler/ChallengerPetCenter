using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Configuration;
using ChallengerPetCenter.DTOs.Auth;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using ChallengerPetCenter.Rules;
using ChallengerPetCenter.Services;
using ChallengerPetCenter.UnitTests.Helpers;
using Microsoft.Extensions.Options;
using Moq;

namespace ChallengerPetCenter.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUnitOfWork> _uow = ServiceMocks.UnitOfWork();

        private AuthService CreateSut() =>
            new(
                _users.Object,
                _uow.Object,
                Options.Create(new JwtSettings
                {
                    Key = "ChallengerPetCenter_Test_Secret_Key_Min32Chars!",
                    Issuer = "ChallengerPetCenter",
                    Audience = "ChallengerPetCenter",
                    ExpirationMinutes = 60
                }));

        [Fact]
        public async Task LoginAsync_QuandoCredenciaisValidas_RetornaToken()
        {
            // Arrange
            _users.Setup(r => r.GetByEmailAsync("ana@email.com")).ReturnsAsync(new User
            {
                Id = 1,
                Nome = "Ana",
                Email = "ana@email.com",
                Senha = "senha123",
                TipoUsuario = "TUTOR",
                Ativo = 1
            });
            var sut = CreateSut();

            // Act
            var result = await sut.LoginAsync(new LoginRequestDto
            {
                Email = "ana@email.com",
                Senha = "senha123"
            });

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(string.IsNullOrWhiteSpace(result.Data!.Token));
            Assert.Equal("ana@email.com", result.Data.Email);
        }

        [Fact]
        public async Task LoginAsync_QuandoSenhaInvalida_RetornaFailure()
        {
            // Arrange
            _users.Setup(r => r.GetByEmailAsync("ana@email.com")).ReturnsAsync(new User
            {
                Id = 1,
                Email = "ana@email.com",
                Senha = "senha123",
                Nome = "Ana",
                TipoUsuario = "TUTOR",
                Ativo = 1
            });
            var sut = CreateSut();

            // Act
            var result = await sut.LoginAsync(new LoginRequestDto
            {
                Email = "ana@email.com",
                Senha = "errada"
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email ou senha inválidos.", result.Error);
        }

        [Fact]
        public async Task LoginAsync_QuandoUsuarioInativo_RetornaFailure()
        {
            // Arrange
            _users.Setup(r => r.GetByEmailAsync("ana@email.com")).ReturnsAsync(new User
            {
                Id = 1,
                Email = "ana@email.com",
                Senha = "senha123",
                Nome = "Ana",
                TipoUsuario = "TUTOR",
                Ativo = 0
            });
            var sut = CreateSut();

            // Act
            var result = await sut.LoginAsync(new LoginRequestDto
            {
                Email = "ana@email.com",
                Senha = "senha123"
            });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(UserRules.InactiveMessage, result.Error);
        }
    }
}
