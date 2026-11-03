using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Comentarios
{
    public class UpdateComentarioDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Texto do comentário é obrigatório.")]
        public string TextoComentario { get; set; } = string.Empty;
    }
}
