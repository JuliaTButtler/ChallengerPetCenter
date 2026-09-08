using System.Diagnostics;
using ChallengerPetCenter.Observability;

namespace ChallengerPetCenter.Observability
{
    public class RequestMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppMetrics _metrics;

        public RequestMetricsMiddleware(RequestDelegate next, AppMetrics metrics)
        {
            _next = next;
            _metrics = metrics;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var path = context.Request.Path.HasValue ? context.Request.Path.Value! : "/";
                _metrics.RecordRequest(
                    context.Request.Method,
                    path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}
