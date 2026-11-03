namespace ChallengerPetCenter.DTOs.Pets
{
    public class PetResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string? Raca { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Observacoes { get; set; }
        public int UserId { get; set; }
    }
}
