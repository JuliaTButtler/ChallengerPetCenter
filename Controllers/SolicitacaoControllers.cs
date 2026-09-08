using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SolicitacoesController : ControllerBase
    {
        private readonly ISolicitacaoService _solicitacaoService;

        public SolicitacoesController(ISolicitacaoService solicitacaoService)
        {
            _solicitacaoService = solicitacaoService;
        }

        /// <summary>
        /// Lista todas as solicitações cadastradas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SolicitacaoResponseDto>>> GetSolicitacoes()
        {
            var solicitacoes = await _solicitacaoService.GetAllAsync();
            return Ok(solicitacoes);
        }

        /// <summary>
        /// Busca uma solicitação pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SolicitacaoResponseDto>> GetSolicitacaoById(int id)
        {
            var result = await _solicitacaoService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca solicitações de um pet.
        /// </summary>
        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SolicitacaoResponseDto>>> GetSolicitacoesByPet(int petId)
        {
            var solicitacoes = (await _solicitacaoService.GetByPetAsync(petId)).ToList();
            if (!solicitacoes.Any())
                return NotFound("Nenhuma solicitação encontrada.");

            return Ok(solicitacoes);
        }

        /// <summary>
        /// Busca solicitações de um tutor.
        /// </summary>
        [HttpGet("tutor/{tutorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SolicitacaoResponseDto>>> GetSolicitacoesByTutor(int tutorId)
        {
            var solicitacoes = (await _solicitacaoService.GetByTutorAsync(tutorId)).ToList();
            if (!solicitacoes.Any())
                return NotFound("Nenhuma solicitação encontrada.");

            return Ok(solicitacoes);
        }

        /// <summary>
        /// Busca solicitações de um veterinário.
        /// </summary>
        [HttpGet("veterinario/{veterinarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SolicitacaoResponseDto>>> GetSolicitacoesByVeterinario(int veterinarioId)
        {
            var solicitacoes = (await _solicitacaoService.GetByVeterinarioAsync(veterinarioId)).ToList();
            if (!solicitacoes.Any())
                return NotFound("Nenhuma solicitação encontrada.");

            return Ok(solicitacoes);
        }

        /// <summary>
        /// Busca solicitações por status.
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SolicitacaoResponseDto>>> GetSolicitacoesByStatus(string status)
        {
            var solicitacoes = (await _solicitacaoService.GetByStatusAsync(status)).ToList();
            if (!solicitacoes.Any())
                return NotFound("Nenhuma solicitação encontrada.");

            return Ok(solicitacoes);
        }

        /// <summary>
        /// Cria uma nova solicitação.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SolicitacaoResponseDto>> CreateSolicitacao(CreateSolicitacaoDto dto)
        {
            var result = await _solicitacaoService.CreateAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetSolicitacaoById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza uma solicitação existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSolicitacao(int id, UpdateSolicitacaoDto dto)
        {
            var result = await _solicitacaoService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove uma solicitação.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSolicitacao(int id)
        {
            var result = await _solicitacaoService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private ActionResult<T> ToActionResult<T>(ServiceResult<T> result)
        {
            if (!result.Succeeded)
                return result.NotFound ? NotFound(result.Error) : BadRequest(result.Error);

            return Ok(result.Data);
        }

        private IActionResult ToActionResult(ServiceResult result)
        {
            if (!result.Succeeded)
                return result.NotFound ? NotFound(result.Error) : BadRequest(result.Error);

            return NoContent();
        }
    }
}
