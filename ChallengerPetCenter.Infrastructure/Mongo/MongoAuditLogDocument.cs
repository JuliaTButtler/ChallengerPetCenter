using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChallengerPetCenter.Infrastructure.Mongo
{
    internal sealed class MongoAuditLogDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Action { get; set; } = string.Empty;

        public string Entity { get; set; } = string.Empty;

        [BsonIgnoreIfNull]
        public string? EntityId { get; set; }

        [BsonIgnoreIfNull]
        public int? ActorUserId { get; set; }

        public DateTime OccurredAt { get; set; }

        [BsonIgnoreIfNull]
        public string? PayloadJson { get; set; }
    }
}
