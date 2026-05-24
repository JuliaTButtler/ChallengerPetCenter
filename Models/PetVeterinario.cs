using System.Text.Json.Serialization;

namespace ChallengerPetCenter.Models
{
    public class PetVeterinario
    {
        public int Id { get; set; }

        public DateTime DataInicio { get; set; } = DateTime.Now;

        public int Ativo { get; set; } = 1;

        public string? Observacoes { get; set; }

        public int PetId { get; set; }

        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public Pet? Pet { get; set; }

        [JsonIgnore]
        public Veterinario? Veterinario { get; set; }
    }
}