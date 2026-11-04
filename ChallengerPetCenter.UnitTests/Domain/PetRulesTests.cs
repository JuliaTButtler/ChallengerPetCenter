using ChallengerPetCenter.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class PetRulesTests
    {
        [Fact]
        public void HasTutor_QuandoUserIdPositivo_RetornaTrue()
        {
            // Arrange / Act
            var result = PetRules.HasTutor(10);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void HasTutor_QuandoPetSemTutor_RetornaFalse(int userId)
        {
            // Arrange / Act
            var result = PetRules.HasTutor(userId);

            // Assert
            Assert.False(result);
            Assert.Equal("Pet sem tutor.", PetRules.MissingTutorMessage);
        }
    }
}
