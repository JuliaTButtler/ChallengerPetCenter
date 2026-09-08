using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Registros
{
    public class CreateRegistroDto
    {
        [Required(ErrorMessage = "Tipo é obrigatório.")]
        public string Tipo { get; set; } = string.Empty;

        public string? Subtipo { get; set; }

        public decimal? Valor { get; set; }

        public string? Unidade { get; set; }

        public string? Nota { get; set; }

        public DateTime? Horario { get; set; }

        [Required(ErrorMessage = "Entrada do diário é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Entrada do diário é obrigatória.")]
        public int DiarioEntradaId { get; set; }
    }
}
