using ChallengerPetCenter.Rules;

namespace ChallengerPetCenter.UnitTests.Domain
{
    public class DiarioEntradaRulesTests
    {
        [Fact]
        public void ConflictsWithExisting_QuandoMesmoDiaJaExiste_RetornaTrue()
        {
            // Arrange
            var existing = new[]
            {
                new DateTime(2026, 11, 3, 8, 0, 0),
                new DateTime(2026, 11, 2, 10, 0, 0)
            };
            var candidate = new DateTime(2026, 11, 3, 22, 0, 0);

            // Act
            var result = DiarioEntradaRules.ConflictsWithExisting(candidate, existing);

            // Assert
            Assert.True(result);
            Assert.Equal(
                "Já existe uma entrada para este pet nesta data.",
                DiarioEntradaRules.DuplicateSameDayMessage);
        }

        [Fact]
        public void ConflictsWithExisting_QuandoDataLivre_RetornaFalse()
        {
            // Arrange
            var existing = new[] { new DateTime(2026, 11, 1) };
            var candidate = new DateTime(2026, 11, 3);

            // Act
            var result = DiarioEntradaRules.ConflictsWithExisting(candidate, existing);

            // Assert
            Assert.False(result);
        }
    }
}
