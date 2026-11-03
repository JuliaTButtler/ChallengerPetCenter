namespace ChallengerPetCenter.Models
{
    /// <summary>
    /// Registro de auditoria persistido no MongoDB (collection audit_logs).
    /// </summary>
    public class AuditLog
    {
        public string Id { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string Entity { get; set; } = string.Empty;

        public string? EntityId { get; set; }

        public int? ActorUserId { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        public string? PayloadJson { get; set; }
    }
}
