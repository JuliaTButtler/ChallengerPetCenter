using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Registros
{
    public class UpdateRegistroDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório.")]
        public string Tipo { get; set; } = string.Empty;

        public string? Subtipo { get; set; }

        public decimal? Valor { get; set; }

        public string? Unidade { get; set; }

        public string? Nota { get; set; }

        public DateTime? Horario { get; set; }
    }
}
