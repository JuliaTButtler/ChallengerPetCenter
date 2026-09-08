using System.Net;
using ChallengerPetCenter.IntegrationTests.Fixtures;

namespace ChallengerPetCenter.IntegrationTests
{
    [Collection(ApiCollection.Name)]
    public class HealthChecksEndpointTests
    {
        private readonly HttpClient _client;

        public HealthChecksEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Health_DeveRetornarOkSemApiKey()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("status", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HealthLive_DeveRetornarOk()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Metrics_DeveExporEndpointPrometheus()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/metrics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(body));
        }
    }
}
