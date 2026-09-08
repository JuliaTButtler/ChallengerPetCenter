using ChallengerPetCenter.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    public static class ControllerResultExtensions
    {
        public static ActionResult ToActionResult(this ControllerBase controller, ServiceResult result)
        {
            if (result.Success)
            {
                return controller.NoContent();
            }

            return result.ErrorType switch
            {
                ServiceErrorType.NotFound => controller.NotFound(result.Message),
                ServiceErrorType.BadRequest => controller.BadRequest(result.Message),
                _ => controller.BadRequest(result.Message)
            };
        }

        public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
        {
            if (result.Success)
            {
                return controller.Ok(result.Data);
            }

            return result.ErrorType switch
            {
                ServiceErrorType.NotFound => controller.NotFound(result.Message),
                ServiceErrorType.BadRequest => controller.BadRequest(result.Message),
                _ => controller.BadRequest(result.Message)
            };
        }

        public static ActionResult<T> ToCreatedResult<T>(
            this ControllerBase controller,
            ServiceResult<T> result,
            string actionName,
            object routeValues)
        {
            if (result.Success)
            {
                return controller.CreatedAtAction(actionName, routeValues, result.Data);
            }

            return result.ErrorType switch
            {
                ServiceErrorType.NotFound => controller.NotFound(result.Message),
                ServiceErrorType.BadRequest => controller.BadRequest(result.Message),
                _ => controller.BadRequest(result.Message)
            };
        }
    }
}
