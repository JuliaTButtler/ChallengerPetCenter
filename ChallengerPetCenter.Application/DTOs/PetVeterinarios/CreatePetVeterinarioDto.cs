using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.PetVeterinarios
{
    public class CreatePetVeterinarioDto
    {
        [Required(ErrorMessage = "Pet é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pet é obrigatório.")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Veterinário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Veterinário é obrigatório.")]
        public int VeterinarioId { get; set; }

        public DateTime? DataInicio { get; set; }

        public string? Observacoes { get; set; }
    }
}
