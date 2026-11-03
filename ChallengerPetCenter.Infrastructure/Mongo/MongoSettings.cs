namespace ChallengerPetCenter.Infrastructure.Mongo
{
    public class MongoSettings
    {
        public const string SectionName = "Mongo";

        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = "PetCenter";
        public string AuditLogsCollection { get; set; } = "audit_logs";
    }
}
