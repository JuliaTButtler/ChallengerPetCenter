using ChallengerPetCenter.Domain.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class PetRulesTests
    {
        [Fact]
        public void ValidarCriacao_QuandoDadosValidos_DeveRetornarNull()
        {
            // Arrange
            var nome = "Bob";
            var especie = "Cachorro";

            // Act
            var resultado = PetRules.ValidarCriacao(nome, especie);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarCriacao_QuandoNomeVazio_DeveRetornarErro()
        {
            // Arrange
            var nome = " ";
            var especie = "Gato";

            // Act
            var resultado = PetRules.ValidarCriacao(nome, especie);

            // Assert
            Assert.Equal("Nome é obrigatório.", resultado);
        }

        [Fact]
        public void ValidarCriacao_QuandoEspecieVazia_DeveRetornarErro()
        {
            // Arrange
            var nome = "Mimi";
            var especie = "";

            // Act
            var resultado = PetRules.ValidarCriacao(nome, especie);

            // Assert
            Assert.Equal("Espécie é obrigatória.", resultado);
        }
    }
}
