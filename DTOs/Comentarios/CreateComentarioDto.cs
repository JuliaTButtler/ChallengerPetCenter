using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Comentarios
{
    public class CreateComentarioDto
    {
        [Required(ErrorMessage = "Texto do comentário é obrigatório.")]
        public string TextoComentario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Entrada do diário é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Entrada do diário é obrigatória.")]
        public int DiarioEntradaId { get; set; }

        [Required(ErrorMessage = "Usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usuário é obrigatório.")]
        public int UserId { get; set; }
    }
}
