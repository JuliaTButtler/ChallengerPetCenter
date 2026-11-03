using ChallengerPetCenter.Infrastructure.Mongo;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ChallengerPetCenter.Infrastructure.Repositories
{
    public class MongoAuditLogRepository : IAuditLogRepository
    {
        private readonly IMongoCollection<MongoAuditLogDocument> _collection;

        public MongoAuditLogRepository(IMongoClient client, IOptions<MongoSettings> options)
        {
            var settings = options.Value;
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<MongoAuditLogDocument>(settings.AuditLogsCollection);
        }

        public async Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
        {
            var document = new MongoAuditLogDocument
            {
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                ActorUserId = log.ActorUserId,
                OccurredAt = log.OccurredAt,
                PayloadJson = log.PayloadJson
            };

            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            log.Id = document.Id;
        }

        public async Task<(IReadOnlyList<AuditLog> Items, long Total)> QueryAsync(
            string? entity = null,
            string? entityId = null,
            string? action = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoAuditLogDocument>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(entity))
            {
                filter &= Builders<MongoAuditLogDocument>.Filter.Eq(x => x.Entity, entity);
            }

            if (!string.IsNullOrWhiteSpace(entityId))
            {
                filter &= Builders<MongoAuditLogDocument>.Filter.Eq(x => x.EntityId, entityId);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                filter &= Builders<MongoAuditLogDocument>.Filter.Eq(x => x.Action, action);
            }

            var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            var documents = await _collection.Find(filter)
                .SortByDescending(x => x.OccurredAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var items = documents.Select(d => new AuditLog
            {
                Id = d.Id,
                Action = d.Action,
                Entity = d.Entity,
                EntityId = d.EntityId,
                ActorUserId = d.ActorUserId,
                OccurredAt = d.OccurredAt,
                PayloadJson = d.PayloadJson
            }).ToList();

            return (items, total);
        }
    }
}
