namespace ChallengerPetCenter.Middleware
{
    /// <summary>
    /// Autenticação simples por API Key via header X-Api-Key.
    /// Quando ApiSettings:ApiKey estiver vazio, a validação é ignorada (compatibilidade).
    /// </summary>
    public class ApiKeyMiddleware
    {
        public const string HeaderName = "X-Api-Key";
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var configuredKey = _configuration["ApiSettings:ApiKey"];
            var path = context.Request.Path.Value ?? string.Empty;

            var isPublicEndpoint =
                path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(configuredKey) || isPublicEndpoint)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
                providedKey != configuredKey)
            {
                _logger.LogWarning("Requisição não autorizada para {Path}", path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key inválida ou ausente.");
                return;
            }

            await _next(context);
        }
    }
}
