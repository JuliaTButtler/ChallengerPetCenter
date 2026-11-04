namespace ChallengerPetCenter.Rules
{
    public static class DiarioEntradaRules
    {
        public const string DuplicateSameDayMessage =
            "Já existe uma entrada para este pet nesta data.";

        public static bool ConflictsWithExisting(
            DateTime candidate,
            IEnumerable<DateTime> existingDates)
        {
            var day = candidate.Date;
            return existingDates.Any(d => d.Date == day);
        }
    }
}
