using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ChallengerPetCenter.HealthChecks
{
    /// <summary>
    /// Verifica se o processo da API está respondendo (liveness).
    /// </summary>
    public class SelfHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("API ChallengerPetCenter está em execução."));
        }
    }
}
