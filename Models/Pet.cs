namespace ChallengerPetCenter.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Especie { get; set; } = string.Empty;

        public string? Raca { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string? Observacoes { get; set; }

        // FK

        public int UserId { get; set; }

        // Navegação

        public User User { get; set; } = null!;

        public List<DiarioEntrada> DiarioEntradas { get; set; } = new();

        public List<PetVeterinario> PetVeterinarios { get; set; } = new();

        public List<Solicitacao> Solicitacoes { get; set; } = new();
    }
}