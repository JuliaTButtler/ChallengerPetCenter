using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Veterinarios
{
    public class CreateVeterinarioDto
    {
        [Required(ErrorMessage = "CRMV é obrigatório.")]
        public string Crmv { get; set; } = string.Empty;

        public string? Especialidade { get; set; }

        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usuário é obrigatório.")]
        public int UserId { get; set; }
    }
}
