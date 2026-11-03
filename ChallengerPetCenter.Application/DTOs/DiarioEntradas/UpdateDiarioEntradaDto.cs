using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.DiarioEntradas
{
    public class UpdateDiarioEntradaDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Data é obrigatória.")]
        public DateTime Data { get; set; }

        public string? Resumo { get; set; }

        public string? HumorGeral { get; set; }

        public string? Status { get; set; }
    }
}
