using System.Text.Json.Serialization;

namespace ChallengerPetCenter.DTOs.Common
{
    public class PagedResponseDto<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }

        [JsonPropertyName("_links")]
        public IReadOnlyList<LinkDto> Links { get; set; } = Array.Empty<LinkDto>();
    }
}
