namespace ChallengerPetCenter.Exceptions
{
    public sealed class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message)
            : base(message, StatusCodes.Status400BadRequest, "Regra de negócio violada")
        {
        }

        private static class StatusCodes
        {
            public const int Status400BadRequest = 400;
        }
    }
}
