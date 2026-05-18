namespace ChallengerPetCenter.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public string TipoUsuario { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public DateTime? UltimoLogin { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamentos

        public List<Pet> Pets { get; set; } = new();

        public List<Comentario> Comentarios { get; set; } = new();

        public List<Solicitacao> Solicitacoes { get; set; } = new();

        public Veterinario? Veterinario { get; set; }
    }
}