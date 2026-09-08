namespace ChallengerPetCenter.DTOs.PetVeterinarios
{
    public class CreatePetVeterinarioDto
    {
        public DateTime? DataInicio { get; set; }
        public int Ativo { get; set; } = 1;
        public string? Observacoes { get; set; }
        public int PetId { get; set; }
        public int VeterinarioId { get; set; }
    }

    public class UpdatePetVeterinarioDto
    {
        public int Id { get; set; }
        public int Ativo { get; set; }
        public string? Observacoes { get; set; }
    }

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
