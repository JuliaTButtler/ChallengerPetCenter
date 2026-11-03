using System.Text.Json;
using ChallengerPetCenter.DTOs.AuditLogs;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.Extensions.Logging;

namespace ChallengerPetCenter.Services
{
    public class AuditLogService : IAuditLogService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        private readonly IAuditLogRepository _repository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository repository, ILogger<AuditLogService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task WriteAsync(
            string action,
            string entity,
            string? entityId,
            object? payload = null,
            int? actorUserId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var log = new AuditLog
                {
                    Action = action,
                    Entity = entity,
                    EntityId = entityId,
                    ActorUserId = actorUserId,
                    OccurredAt = DateTime.UtcNow,
                    PayloadJson = payload is null
                        ? null
                        : JsonSerializer.Serialize(payload, JsonOptions)
                };

                await _repository.AddAsync(log, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Falha ao gravar audit log {Action} {Entity}/{EntityId}. Operação principal não foi afetada.",
                    action,
                    entity,
                    entityId);
            }
        }

        public async Task<AuditLogListResponseDto> QueryAsync(
            string? entity = null,
            string? entityId = null,
            string? action = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

            var (items, total) = await _repository.QueryAsync(
                entity,
                entityId,
                action,
                page,
                pageSize,
                cancellationToken);

            return new AuditLogListResponseDto
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items.Select(MapToDto).ToList()
            };
        }

        private static AuditLogResponseDto MapToDto(AuditLog log)
        {
            object? payload = null;
            if (!string.IsNullOrWhiteSpace(log.PayloadJson))
            {
                try
                {
                    payload = JsonSerializer.Deserialize<JsonElement>(log.PayloadJson);
                }
                catch
                {
                    payload = log.PayloadJson;
                }
            }

            return new AuditLogResponseDto
            {
                Id = log.Id,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                ActorUserId = log.ActorUserId,
                OccurredAt = log.OccurredAt,
                Payload = payload
            };
        }
    }
}
