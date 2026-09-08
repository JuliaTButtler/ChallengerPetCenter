using ChallengerPetCenter.Domain.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class DiarioEntradaRulesTests
    {
        [Fact]
        public void ValidarCriacao_QuandoPetExisteEDataLivre_DeveRetornarNull()
        {
            // Arrange
            var data = DateTime.Today;
            var petExiste = true;
            var jaExisteEntrada = false;

            // Act
            var resultado = DiarioEntradaRules.ValidarCriacao(data, petExiste, jaExisteEntrada);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarCriacao_QuandoPetNaoExiste_DeveRetornarErro()
        {
            // Arrange
            var data = DateTime.Today;

            // Act
            var resultado = DiarioEntradaRules.ValidarCriacao(data, petExiste: false, jaExisteEntradaNaData: false);

            // Assert
            Assert.Equal("Pet não encontrado.", resultado);
        }

        [Fact]
        public void ValidarCriacao_QuandoJaExisteEntradaNaData_DeveRetornarErro()
        {
            // Arrange
            var data = DateTime.Today;

            // Act
            var resultado = DiarioEntradaRules.ValidarCriacao(data, petExiste: true, jaExisteEntradaNaData: true);

            // Assert
            Assert.Equal("Já existe uma entrada para este pet nesta data.", resultado);
        }

        [Fact]
        public void PodeCriarEntradaParaData_QuandoNaoExiste_DeveRetornarTrue()
        {
            // Arrange
            var jaExiste = false;

            // Act
            var resultado = DiarioEntradaRules.PodeCriarEntradaParaData(jaExiste);

            // Assert
            Assert.True(resultado);
        }
    }
}
