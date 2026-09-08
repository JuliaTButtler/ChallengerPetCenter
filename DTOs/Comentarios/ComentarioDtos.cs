namespace ChallengerPetCenter.DTOs.Comentarios
{
    public class CreateComentarioDto
    {
        public string TextoComentario { get; set; } = string.Empty;
        public int DiarioEntradaId { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateComentarioDto
    {
        public int Id { get; set; }
        public string TextoComentario { get; set; } = string.Empty;
    }

    public class ComentarioResponseDto
    {
        public int Id { get; set; }
        public string TextoComentario { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
        public DateTime? EditadoEm { get; set; }
        public int DiarioEntradaId { get; set; }
        public int UserId { get; set; }
    }
}
