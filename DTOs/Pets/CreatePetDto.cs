using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Pets
{
    public class CreatePetDto
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Espécie é obrigatória.")]
        public string Especie { get; set; } = string.Empty;

        public string? Raca { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string? Observacoes { get; set; }

        [Required(ErrorMessage = "Tutor é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tutor é obrigatório.")]
        public int UserId { get; set; }
    }
}
