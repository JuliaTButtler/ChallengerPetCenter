using ChallengerPetCenter.Domain.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class UserRulesTests
    {
        [Fact]
        public void ValidarCriacao_QuandoDadosValidos_DeveRetornarNull()
        {
            // Arrange
            var nome = "Maria Tutor";
            var email = "maria@email.com";
            var tipo = "TUTOR";

            // Act
            var resultado = UserRules.ValidarCriacao(nome, email, tipo);

            // Assert
            Assert.Null(resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ValidarCriacao_QuandoNomeInvalido_DeveRetornarErro(string? nome)
        {
            // Arrange
            var email = "maria@email.com";
            var tipo = "TUTOR";

            // Act
            var resultado = UserRules.ValidarCriacao(nome, email, tipo);

            // Assert
            Assert.Equal("Nome é obrigatório.", resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sem-arroba")]
        public void ValidarCriacao_QuandoEmailInvalido_DeveRetornarErro(string? email)
        {
            // Arrange
            var nome = "Maria";
            var tipo = "TUTOR";

            // Act
            var resultado = UserRules.ValidarCriacao(nome, email, tipo);

            // Assert
            Assert.Equal("Email é obrigatório.", resultado);
        }

        [Theory]
        [InlineData("ADMIN")]
        [InlineData("")]
        [InlineData(null)]
        public void ValidarCriacao_QuandoTipoInvalido_DeveRetornarErro(string? tipo)
        {
            // Arrange
            var nome = "Maria";
            var email = "maria@email.com";

            // Act
            var resultado = UserRules.ValidarCriacao(nome, email, tipo);

            // Assert
            Assert.Equal("Tipo de usuário inválido. Use TUTOR ou VETERINARIO.", resultado);
        }

        [Fact]
        public void IsTipoUsuarioValido_QuandoVeterinario_DeveRetornarTrue()
        {
            // Arrange
            var tipo = "veterinario";

            // Act
            var resultado = UserRules.IsTipoUsuarioValido(tipo);

            // Assert
            Assert.True(resultado);
        }
    }
}
