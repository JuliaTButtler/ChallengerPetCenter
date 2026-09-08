namespace ChallengerPetCenter.DTOs.Solicitacoes
{
    public class SolicitacaoResponseDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Mensagem { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? RespondidoEm { get; set; }
        public int PetId { get; set; }
        public int TutorId { get; set; }
        public int VeterinarioId { get; set; }
    }
}
