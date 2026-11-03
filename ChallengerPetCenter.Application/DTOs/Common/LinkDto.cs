using System.Text.Json.Serialization;

namespace ChallengerPetCenter.DTOs.Common
{
    public class LinkDto
    {
        [JsonPropertyName("rel")]
        public string Rel { get; set; } = string.Empty;

        [JsonPropertyName("href")]
        public string Href { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string Method { get; set; } = "GET";
    }
}
