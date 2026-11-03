using ChallengerPetCenter.DTOs.AuditLogs;

namespace ChallengerPetCenter.Services
{
    /// <summary>
    /// Auditoria desabilitada (testes / Mongo não configurado).
    /// </summary>
    public sealed class NoOpAuditLogService : IAuditLogService
    {
        public Task WriteAsync(
            string action,
            string entity,
            string? entityId,
            object? payload = null,
            int? actorUserId = null,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<AuditLogListResponseDto> QueryAsync(
            string? entity = null,
            string? entityId = null,
            string? action = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new AuditLogListResponseDto
            {
                Page = page < 1 ? 1 : page,
                PageSize = pageSize,
                Total = 0,
                Items = Array.Empty<AuditLogResponseDto>()
            });
    }
}
