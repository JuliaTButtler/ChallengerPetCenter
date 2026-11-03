namespace ChallengerPetCenter.DTOs.DiarioEntradas
{
    public class DiarioEntradaResponseDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public string? Resumo { get; set; }
        public string? HumorGeral { get; set; }
        public string? Status { get; set; }
        public int PetId { get; set; }
    }
}
