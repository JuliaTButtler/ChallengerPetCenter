using System.Text.Json.Serialization;

namespace ChallengerPetCenter.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        public string TextoComentario { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public DateTime? EditadoEm { get; set; }

        public int DiarioEntradaId { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public DiarioEntrada? DiarioEntrada { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
    }
}