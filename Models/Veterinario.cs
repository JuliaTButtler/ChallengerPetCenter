using System.Text.Json.Serialization;

namespace ChallengerPetCenter.Models
{
    public class Veterinario
    {
        public int Id { get; set; }

        public string Crmv { get; set; } = string.Empty;

        public string? Especialidade { get; set; }

        public string? Descricao { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public List<PetVeterinario> PetVeterinarios { get; set; } = new();

        [JsonIgnore]
        public List<Solicitacao> Solicitacoes { get; set; } = new();
    }
}