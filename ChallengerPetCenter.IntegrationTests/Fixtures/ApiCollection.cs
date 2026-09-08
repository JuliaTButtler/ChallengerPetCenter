using ChallengerPetCenter.IntegrationTests.Fixtures;
using Xunit;

namespace ChallengerPetCenter.IntegrationTests.Fixtures
{
    [CollectionDefinition(Name)]
    public class ApiCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
        public const string Name = "ApiCollection";
    }
}
