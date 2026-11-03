using System.ComponentModel.DataAnnotations;

namespace ChallengerPetCenter.DTOs.DiarioEntradas
{
    public class CreateDiarioEntradaDto
    {
        [Required(ErrorMessage = "Data é obrigatória.")]
        public DateTime Data { get; set; }

        public string? Resumo { get; set; }

        public string? HumorGeral { get; set; }

        public string? Status { get; set; }

        [Required(ErrorMessage = "Pet é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pet é obrigatório.")]
        public int PetId { get; set; }
    }
}
