using System.Text.Json.Serialization;

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

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;

        [JsonIgnore]
        public List<DiarioEntrada> DiarioEntradas { get; set; } = new();

        [JsonIgnore]
        public List<PetVeterinario> PetVeterinarios { get; set; } = new();

        [JsonIgnore]
        public List<Solicitacao> Solicitacoes { get; set; } = new();
    }
}