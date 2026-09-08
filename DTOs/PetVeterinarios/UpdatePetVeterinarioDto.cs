using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.PetVeterinarios
{
    public class UpdatePetVeterinarioDto
    {
        [Required]
        public int Id { get; set; }

        public int Ativo { get; set; }

        public string? Observacoes { get; set; }
    }
}
