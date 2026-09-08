using System.Diagnostics.Metrics;

namespace ChallengerPetCenter.Observability
{
    /// <summary>
    /// Métricas customizadas da aplicação (requisições e erros).
    /// </summary>
    public sealed class AppMetrics
    {
        public const string MeterName = "ChallengerPetCenter";

        private readonly Counter<long> _requestCounter;
        private readonly Counter<long> _errorCounter;
        private readonly Histogram<double> _responseTimeMs;

        public AppMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create(MeterName);

            _requestCounter = meter.CreateCounter<long>(
                "petcenter.requests",
                description: "Total de requisições HTTP processadas");

            _errorCounter = meter.CreateCounter<long>(
                "petcenter.errors",
                description: "Total de respostas HTTP com erro (status >= 400)");

            _responseTimeMs = meter.CreateHistogram<double>(
                "petcenter.response_time_ms",
                unit: "ms",
                description: "Tempo de resposta das requisições HTTP em milissegundos");
        }

        public void RecordRequest(string method, string path, int statusCode, double elapsedMs)
        {
            var tags = new KeyValuePair<string, object?>[]
            {
                new("http.method", method),
                new("http.route", path),
                new("http.status_code", statusCode)
            };

            _requestCounter.Add(1, tags);
            _responseTimeMs.Record(elapsedMs, tags);

            if (statusCode >= 400)
            {
                _errorCounter.Add(1, tags);
            }
        }
    }
}
