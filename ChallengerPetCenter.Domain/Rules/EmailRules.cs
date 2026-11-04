using System.Text.RegularExpressions;

namespace ChallengerPetCenter.Rules
{
    public static class EmailRules
    {
        public const string InvalidMessage = "Email inválido.";

        private static readonly Regex Pattern = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static bool IsValid(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Pattern.IsMatch(email.Trim());
        }
    }
}
