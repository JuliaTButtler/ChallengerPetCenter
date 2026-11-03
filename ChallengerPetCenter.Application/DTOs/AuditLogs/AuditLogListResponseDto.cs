namespace ChallengerPetCenter.DTOs.AuditLogs
{
    public class AuditLogListResponseDto
    {
        public IReadOnlyList<AuditLogResponseDto> Items { get; set; } = Array.Empty<AuditLogResponseDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
    }
}
