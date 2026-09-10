using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ChallengerPetCenter.HealthChecks
{
    public class ExternalServiceHealthCheckOptions
    {
        public const string SectionName = "HealthChecks";

        /// <summary>
        /// URL externa a ser consultada (ex.: https://www.google.com).
        /// Se vazia, o check reporta Healthy com observação de que não há serviço configurado.
        /// </summary>
        public string? ExternalServiceUrl { get; set; }
    }

    /// <summary>
    /// Verifica disponibilidade de um serviço externo via HTTP GET.
    /// </summary>
    public class ExternalServiceHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExternalServiceHealthCheckOptions _options;

        public ExternalServiceHealthCheck(
            IHttpClientFactory httpClientFactory,
            IOptions<ExternalServiceHealthCheckOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ExternalServiceUrl))
            {
                return HealthCheckResult.Healthy(
                    "Nenhum serviço externo configurado (HealthChecks:ExternalServiceUrl).");
            }

            try
            {
                var client = _httpClientFactory.CreateClient(nameof(ExternalServiceHealthCheck));
                using var response = await client.GetAsync(
                    _options.ExternalServiceUrl,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy(
                        $"Serviço externo respondendo: {_options.ExternalServiceUrl} ({(int)response.StatusCode}).");
                }

                return HealthCheckResult.Degraded(
                    $"Serviço externo retornou {(int)response.StatusCode}: {_options.ExternalServiceUrl}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    $"Falha ao consultar serviço externo: {_options.ExternalServiceUrl}",
                    ex);
            }
        }
    }
}
