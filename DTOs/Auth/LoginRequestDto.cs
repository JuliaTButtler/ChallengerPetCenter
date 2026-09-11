using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string Senha { get; set; } = string.Empty;
    }
}
