using ChallengerPetCenter.DTOs.AuditLogs;

namespace ChallengerPetCenter.Services
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Grava auditoria em best-effort (falha no Mongo não propaga para a operação principal).
        /// </summary>
        Task WriteAsync(
            string action,
            string entity,
            string? entityId,
            object? payload = null,
            int? actorUserId = null,
            CancellationToken cancellationToken = default);

        Task<AuditLogListResponseDto> QueryAsync(
            string? entity = null,
            string? entityId = null,
            string? action = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
    }
}
