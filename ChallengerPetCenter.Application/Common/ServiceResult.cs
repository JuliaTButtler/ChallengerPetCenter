namespace ChallengerPetCenter.Common
{
    public class ServiceResult
    {
        public bool Succeeded { get; init; }
        public string? Error { get; init; }
        public bool NotFound { get; init; }

        public static ServiceResult Success() =>
            new() { Succeeded = true };

        public static ServiceResult Failure(string error) =>
            new() { Succeeded = false, Error = error };

        public static ServiceResult AsNotFound(string error) =>
            new() { Succeeded = false, NotFound = true, Error = error };
    }

    public class ServiceResult<T>
    {
        public bool Succeeded { get; init; }
        public T? Data { get; init; }
        public string? Error { get; init; }
        public bool NotFound { get; init; }

        public static ServiceResult<T> Success(T data) =>
            new() { Succeeded = true, Data = data };

        public static ServiceResult<T> Failure(string error) =>
            new() { Succeeded = false, Error = error };

        public static ServiceResult<T> AsNotFound(string error) =>
            new() { Succeeded = false, NotFound = true, Error = error };
    }
}
