namespace ChallengerPetCenter.Rules
{
    public static class PetRules
    {
        public const string MissingTutorMessage = "Pet sem tutor.";

        public static bool HasTutor(int userId) => userId > 0;
    }
}
