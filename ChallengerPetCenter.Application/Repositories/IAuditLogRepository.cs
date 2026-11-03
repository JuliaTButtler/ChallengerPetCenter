using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<AuditLog> Items, long Total)> QueryAsync(
            string? entity = null,
            string? entityId = null,
            string? action = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
    }
}
