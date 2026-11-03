using ChallengerPetCenter.Common;

namespace ChallengerPetCenter.UnitTests.Common
{
    public class PagingHelperTests
    {
        [Fact]
        public void Page_QuandoPageInvalida_NormalizaParaUm()
        {
            // Arrange
            var source = Enumerable.Range(1, 5);

            // Act
            var (items, page, pageSize, total) = PagingHelper.Page(source, page: 0, pageSize: 2);

            // Assert
            Assert.Equal(1, page);
            Assert.Equal(2, pageSize);
            Assert.Equal(5, total);
            Assert.Equal(new[] { 1, 2 }, items);
        }

        [Fact]
        public void ApplySort_QuandoOrderDesc_OrdenaDecrescente()
        {
            // Arrange
            var source = new[] { "b", "a", "c" };

            // Act
            var result = PagingHelper.ApplySort(source, x => x, "desc").ToList();

            // Assert
            Assert.Equal(new[] { "c", "b", "a" }, result);
        }
    }
}
