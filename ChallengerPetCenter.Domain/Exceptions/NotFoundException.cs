namespace ChallengerPetCenter.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound, "Recurso não encontrado")
        {
        }

        // Evita dependência de ASP.NET no Domain: constantes locais.
        private static class StatusCodes
        {
            public const int Status404NotFound = 404;
        }
    }
}
