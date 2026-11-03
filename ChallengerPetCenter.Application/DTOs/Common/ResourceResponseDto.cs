using System.Text.Json.Serialization;

namespace ChallengerPetCenter.DTOs.Common
{
    public class ResourceResponseDto<T>
    {
        public T Data { get; set; } = default!;

        [JsonPropertyName("_links")]
        public IReadOnlyList<LinkDto> Links { get; set; } = Array.Empty<LinkDto>();
    }
}
