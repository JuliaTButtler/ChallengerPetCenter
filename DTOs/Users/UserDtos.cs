namespace ChallengerPetCenter.DTOs.Users
{
    public class CreateUserDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public int Ativo { get; set; } = 1;
    }

    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public int Ativo { get; set; } = 1;
    }

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
