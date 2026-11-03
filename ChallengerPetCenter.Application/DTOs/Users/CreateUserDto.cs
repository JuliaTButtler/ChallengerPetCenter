using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.Users
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string Senha { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Tipo de usuário é obrigatório.")]
        public string TipoUsuario { get; set; } = string.Empty;
    }
}
