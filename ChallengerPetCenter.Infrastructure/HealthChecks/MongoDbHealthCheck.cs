using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChallengerPetCenter.Infrastructure.HealthChecks
{
    public sealed class MongoDbHealthCheck : IHealthCheck
    {
        private readonly IMongoClient _client;

        public MongoDbHealthCheck(IMongoClient client)
        {
            _client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _client.GetDatabase("admin");
                await database.RunCommandAsync(
                    (Command<BsonDocument>)"{ping:1}",
                    cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy("MongoDB respondendo ao ping.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Falha ao conectar no MongoDB.", ex);
            }
        }
    }
}
