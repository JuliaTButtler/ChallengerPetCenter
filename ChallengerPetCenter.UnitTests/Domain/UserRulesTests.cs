using ChallengerPetCenter.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class UserRulesTests
    {
        [Fact]
        public void IsActive_QuandoAtivoIgualUm_RetornaTrue()
        {
            // Arrange / Act
            var result = UserRules.IsActive(1);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(-1)]
        public void IsActive_QuandoUsuarioInativo_RetornaFalse(int ativo)
        {
            // Arrange / Act
            var result = UserRules.IsActive(ativo);

            // Assert
            Assert.False(result);
        }
    }
}
