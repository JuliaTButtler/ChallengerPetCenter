namespace ChallengerPetCenter.Models
{
    public class PetVeterinario
    {
        public int Id { get; set; }

        public DateTime DataInicio { get; set; } = DateTime.Now;

        public bool Ativo { get; set; } = true;

        public string? Observacoes { get; set; }

        // FK

        public int PetId { get; set; }

        public int VeterinarioId { get; set; }

        // Navegação

        public Pet Pet { get; set; } = null!;

        public Veterinario Veterinario { get; set; } = null!;
    }
}