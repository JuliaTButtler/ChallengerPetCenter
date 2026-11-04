using System.Net;
using ChallengerPetCenter.IntegrationTests.Fixtures;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection("Integration collection")]
    public class HealthIntegrationTests
    {
        private readonly HttpClient _client;

        public HealthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthLive_QuandoApiSobe_RetornaOk()
        {
            // Arrange / Act
            var response = await _client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task HealthReady_QuandoDependenciasOk_RetornaOk()
        {
            // Arrange / Act
            var response = await _client.GetAsync("/health/ready");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", body, StringComparison.OrdinalIgnoreCase);
        }
    }
}
