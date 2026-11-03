namespace ChallengerPetCenter.DTOs.Veterinarios
{
    public class VeterinarioResponseDto
    {
        public int Id { get; set; }
        public string Crmv { get; set; } = string.Empty;
        public string? Especialidade { get; set; }
        public string? Descricao { get; set; }
        public int UserId { get; set; }
    }
}
