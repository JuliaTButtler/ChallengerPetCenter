using ChallengerPetCenter.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.API.Exceptions
{
    /// <summary>
    /// Tratamento global de exceções com resposta ProblemDetails (RFC 7807).
    /// </summary>
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = MapException(exception);

            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Erro não tratado em {Method} {Path}",
                    httpContext.Request.Method,
                    httpContext.Request.Path);
            }
            else
            {
                _logger.LogWarning(exception, "Falha de negócio/requisição em {Method} {Path}: {Detail}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    detail);
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            if (_environment.IsDevelopment() && statusCode >= 500)
            {
                problem.Extensions["exception"] = exception.GetType().Name;
                problem.Extensions["stackTrace"] = exception.StackTrace;
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken: cancellationToken);

            return true;
        }

        private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
        {
            return exception switch
            {
                AppException app => (app.StatusCode, app.Title, app.Message),
                UnauthorizedAccessException ua => (StatusCodes.Status401Unauthorized, "Não autorizado", ua.Message),
                ArgumentException arg => (StatusCodes.Status400BadRequest, "Requisição inválida", arg.Message),
                KeyNotFoundException knf => (StatusCodes.Status404NotFound, "Recurso não encontrado", knf.Message),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Erro interno do servidor",
                    "Ocorreu um erro inesperado. Tente novamente mais tarde.")
            };
        }
    }
}
