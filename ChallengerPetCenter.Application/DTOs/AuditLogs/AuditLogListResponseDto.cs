using System.Text.Json.Serialization;
using ChallengerPetCenter.DTOs.Common;

namespace ChallengerPetCenter.DTOs.AuditLogs
{
    public class AuditLogListResponseDto
    {
        public IReadOnlyList<AuditLogResponseDto> Items { get; set; } = Array.Empty<AuditLogResponseDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }

        [JsonPropertyName("_links")]
        public IReadOnlyList<LinkDto> Links { get; set; } = Array.Empty<LinkDto>();
    }
}
