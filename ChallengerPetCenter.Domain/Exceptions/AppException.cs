namespace ChallengerPetCenter.Exceptions
{
    /// <summary>
    /// Exceção base da aplicação, com status HTTP associado.
    /// </summary>
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string Title { get; }

        protected AppException(string message, int statusCode, string title)
            : base(message)
        {
            StatusCode = statusCode;
            Title = title;
        }
    }
}
