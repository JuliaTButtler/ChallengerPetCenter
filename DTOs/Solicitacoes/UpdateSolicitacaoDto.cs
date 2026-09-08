using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Solicitacoes
{
    public class UpdateSolicitacaoDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Status é obrigatório.")]
        public string Status { get; set; } = string.Empty;

        public string? Mensagem { get; set; }

        public DateTime? RespondidoEm { get; set; }
    }
}
