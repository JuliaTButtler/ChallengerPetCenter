using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Solicitacoes
{
    public class CreateSolicitacaoDto
    {
        public string? Status { get; set; }

        public string? Mensagem { get; set; }

        [Required(ErrorMessage = "Pet é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pet é obrigatório.")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Tutor é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tutor é obrigatório.")]
        public int TutorId { get; set; }

        [Required(ErrorMessage = "Veterinário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Veterinário é obrigatório.")]
        public int VeterinarioId { get; set; }
    }
}
