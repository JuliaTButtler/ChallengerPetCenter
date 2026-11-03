using ChallengerPetCenter.Common;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    /// <summary>
    /// Base dos controllers: mapeia ServiceResult e erros para ProblemDetails.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult<T> ToActionResult<T>(ServiceResult<T> result)
        {
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Recurso não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(result.Data);
        }

        protected IActionResult ToActionResult(ServiceResult result)
        {
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Recurso não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return NoContent();
        }

        protected ObjectResult ProblemNotFound(string detail)
            => ProblemResult(
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                detail);

        protected ObjectResult ProblemBadRequest(string detail)
            => ProblemResult(
                StatusCodes.Status400BadRequest,
                "Requisição inválida",
                detail);

        private ObjectResult ProblemResult(int statusCode, string title, string detail)
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = HttpContext?.Request.Path.Value
            };

            if (HttpContext is not null)
            {
                problem.Extensions["traceId"] = HttpContext.TraceIdentifier;
            }

            return StatusCode(statusCode, problem);
        }
    }
}
