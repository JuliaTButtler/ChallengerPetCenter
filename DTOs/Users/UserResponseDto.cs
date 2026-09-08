namespace ChallengerPetCenter.DTOs.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public int Ativo { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
