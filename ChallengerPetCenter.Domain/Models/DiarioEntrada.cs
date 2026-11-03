using System.Text.Json.Serialization;

namespace ChallengerPetCenter.Models
{
    public class DiarioEntrada
    {
        public int Id { get; set; }

        public DateTime Data { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public DateTime? AtualizadoEm { get; set; }

        public string? Resumo { get; set; }

        public string? HumorGeral { get; set; }

        public string? Status { get; set; }

        public int PetId { get; set; }

        [JsonIgnore]
        public Pet? Pet { get; set; }

        [JsonIgnore]
        public List<Registro> Registros { get; set; } = new();

        [JsonIgnore]
        public List<Comentario> Comentarios { get; set; } = new();
    }
}