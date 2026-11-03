namespace ChallengerPetCenter.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = "ChallengerPetCenter";
        public string Audience { get; set; } = "ChallengerPetCenter";
        public int ExpirationMinutes { get; set; } = 60;
    }
}
