using System.Net;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ChallengerPetCenter.HealthChecks
{
    /// <summary>
    /// Verifica a disponibilidade de um serviço externo configurado via URL.
    /// </summary>
    public class ExternalServiceHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalServiceHealthCheck> _logger;

        public ExternalServiceHealthCheck(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ExternalServiceHealthCheck> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var url = _configuration["ExternalServices:HealthCheckUrl"];

            if (string.IsNullOrWhiteSpace(url))
            {
                return HealthCheckResult.Degraded(
                    "URL do serviço externo não configurada em ExternalServices:HealthCheckUrl.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ExternalHealthCheck");
                using var response = await client.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
                {
                    return HealthCheckResult.Healthy($"Serviço externo respondeu com {(int)response.StatusCode}.");
                }

                _logger.LogWarning(
                    "Health check do serviço externo retornou {StatusCode} para {Url}",
                    (int)response.StatusCode,
                    url);

                return HealthCheckResult.Unhealthy(
                    $"Serviço externo indisponível. Status: {(int)response.StatusCode}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao consultar serviço externo {Url}", url);
                return HealthCheckResult.Unhealthy("Falha ao consultar serviço externo.", ex);
            }
        }
    }
}
