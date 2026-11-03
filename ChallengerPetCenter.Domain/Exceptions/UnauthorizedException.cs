namespace ChallengerPetCenter.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Não autenticado.")
            : base(message, StatusCodes.Status401Unauthorized, "Não autorizado")
        {
        }

        private static class StatusCodes
        {
            public const int Status401Unauthorized = 401;
        }
    }
}
