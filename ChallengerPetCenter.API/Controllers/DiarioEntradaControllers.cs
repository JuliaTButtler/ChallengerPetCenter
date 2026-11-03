using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiarioEntradasController : ApiControllerBase
    {
        private readonly IDiarioEntradaService _diarioEntradaService;

        public DiarioEntradasController(IDiarioEntradaService diarioEntradaService)
        {
            _diarioEntradaService = diarioEntradaService;
        }

        /// <summary>
        /// Lista todas as entradas do diário.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DiarioEntradaResponseDto>>> GetDiarioEntradas()
        {
            var entradas = await _diarioEntradaService.GetAllAsync();
            return Ok(entradas);
        }

        /// <summary>
        /// Busca uma entrada pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DiarioEntradaResponseDto>> GetDiarioEntradaById(int id)
        {
            var result = await _diarioEntradaService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca entradas de um pet.
        /// </summary>
        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DiarioEntradaResponseDto>>> GetEntradasByPet(int petId)
        {
            var entradas = (await _diarioEntradaService.GetByPetAsync(petId)).ToList();
            if (!entradas.Any())
                return ProblemNotFound("Nenhuma entrada encontrada.");

            return Ok(entradas);
        }

        /// <summary>
        /// Busca entradas por status.
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DiarioEntradaResponseDto>>> GetEntradasByStatus(string status)
        {
            var entradas = (await _diarioEntradaService.GetByStatusAsync(status)).ToList();
            if (!entradas.Any())
                return ProblemNotFound("Nenhuma entrada encontrada.");

            return Ok(entradas);
        }

        /// <summary>
        /// Busca entradas por humor geral.
        /// </summary>
        [HttpGet("humor/{humor}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DiarioEntradaResponseDto>>> GetEntradasByHumor(string humor)
        {
            var entradas = (await _diarioEntradaService.GetByHumorAsync(humor)).ToList();
            if (!entradas.Any())
                return ProblemNotFound("Nenhuma entrada encontrada.");

            return Ok(entradas);
        }

        /// <summary>
        /// Cria uma nova entrada no diário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DiarioEntradaResponseDto>> CreateDiarioEntrada(CreateDiarioEntradaDto dto)
        {
            var result = await _diarioEntradaService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            return CreatedAtAction(
                nameof(GetDiarioEntradaById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza uma entrada do diário.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDiarioEntrada(int id, UpdateDiarioEntradaDto dto)
        {
            var result = await _diarioEntradaService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove uma entrada do diário.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDiarioEntrada(int id)
        {
            var result = await _diarioEntradaService.DeleteAsync(id);
            return ToActionResult(result);
        }
    }
}
