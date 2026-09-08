namespace ChallengerPetCenter.Services.Common
{
    public enum ServiceErrorType
    {
        None,
        NotFound,
        BadRequest
    }

    public class ServiceResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public ServiceErrorType ErrorType { get; init; }

        public static ServiceResult Ok(string? message = null) => new()
        {
            Success = true,
            Message = message,
            ErrorType = ServiceErrorType.None
        };

        public static ServiceResult NotFound(string message) => new()
        {
            Success = false,
            Message = message,
            ErrorType = ServiceErrorType.NotFound
        };

        public static ServiceResult BadRequest(string message) => new()
        {
            Success = false,
            Message = message,
            ErrorType = ServiceErrorType.BadRequest
        };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message,
            ErrorType = ServiceErrorType.None
        };

        public new static ServiceResult<T> NotFound(string message) => new()
        {
            Success = false,
            Message = message,
            ErrorType = ServiceErrorType.NotFound
        };

        public new static ServiceResult<T> BadRequest(string message) => new()
        {
            Success = false,
            Message = message,
            ErrorType = ServiceErrorType.BadRequest
        };
    }
}
