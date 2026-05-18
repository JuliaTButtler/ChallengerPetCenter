namespace ChallengerPetCenter.Models
{
    public class Veterinario
    {
        public int Id { get; set; }

        public string Crmv { get; set; } = string.Empty;

        public string? Especialidade { get; set; }

        public string? Descricao { get; set; }

        // FK

        public int UserId { get; set; }

        // Navegação

        public User User { get; set; } = null!;

        public List<PetVeterinario> PetVeterinarios { get; set; } = new();

        public List<Solicitacao> Solicitacoes { get; set; } = new();
    }
}