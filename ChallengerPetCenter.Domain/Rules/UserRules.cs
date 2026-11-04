namespace ChallengerPetCenter.Rules
{
    public static class UserRules
    {
        public const string InactiveMessage = "Usuário inativo.";

        public static bool IsActive(int ativo) => ativo == 1;
    }
}
