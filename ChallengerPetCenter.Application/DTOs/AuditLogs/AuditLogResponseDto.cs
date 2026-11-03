namespace ChallengerPetCenter.DTOs.AuditLogs
{
    public class AuditLogResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public int? ActorUserId { get; set; }
        public DateTime OccurredAt { get; set; }
        public object? Payload { get; set; }
    }
}
