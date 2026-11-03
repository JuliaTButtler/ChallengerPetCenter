using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Veterinarios
{
    public class UpdateVeterinarioDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "CRMV é obrigatório.")]
        public string Crmv { get; set; } = string.Empty;

        public string? Especialidade { get; set; }

        public string? Descricao { get; set; }
    }
}
