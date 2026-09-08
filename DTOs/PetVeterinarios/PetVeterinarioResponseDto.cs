namespace ChallengerPetCenter.DTOs.PetVeterinarios
{
    public class PetVeterinarioResponseDto
    {
        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public int Ativo { get; set; }
        public string? Observacoes { get; set; }
        public int PetId { get; set; }
        public int VeterinarioId { get; set; }
    }
}
