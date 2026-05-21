using System.Text.Json.Serialization;

namespace ChallengerPetCenter.Models
{
    public class Registro
    {
        public int Id { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string? Subtipo { get; set; }

        public decimal? Valor { get; set; }

        public string? Unidade { get; set; }

        public string? Nota { get; set; }

        public DateTime? Horario { get; set; }

        public DateTime? AtualizadoEm { get; set; }

        public int DiarioEntradaId { get; set; }

        [JsonIgnore]
        public DiarioEntrada DiarioEntrada { get; set; } = null!;
    }
}