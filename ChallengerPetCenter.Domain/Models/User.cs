using System.Text.Json.Serialization;

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

        public int Ativo { get; set; } = 1;

        public DateTime? UltimoLogin { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamentos

        [JsonIgnore]
        public List<Pet> Pets { get; set; } = new();

        [JsonIgnore]
        public List<Comentario> Comentarios { get; set; } = new();

        [JsonIgnore]
        public List<Solicitacao> Solicitacoes { get; set; } = new();

        [JsonIgnore]
        public Veterinario? Veterinario { get; set; }
    }
}