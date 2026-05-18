namespace ChallengerPetCenter.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Mensagem { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public DateTime? RespondidoEm { get; set; }

        // FK

        public int PetId { get; set; }

        public int TutorId { get; set; }

        public int VeterinarioId { get; set; }

        // Navegação

        public Pet Pet { get; set; } = null!;

        public User Tutor { get; set; } = null!;

        public Veterinario Veterinario { get; set; } = null!;
    }
}