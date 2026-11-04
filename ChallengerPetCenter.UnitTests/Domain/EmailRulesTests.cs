using ChallengerPetCenter.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class EmailRulesTests
    {
        [Theory]
        [InlineData("ana@email.com")]
        [InlineData("tutor.pet@dominio.com.br")]
        public void IsValid_QuandoEmailBemFormado_RetornaTrue(string email)
        {
            // Arrange / Act
            var result = EmailRules.IsValid(email);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("sem-arroba")]
        [InlineData("a@b")]
        [InlineData("@email.com")]
        public void IsValid_QuandoEmailInvalido_RetornaFalse(string? email)
        {
            // Arrange / Act
            var result = EmailRules.IsValid(email);

            // Assert
            Assert.False(result);
        }
    }
}
