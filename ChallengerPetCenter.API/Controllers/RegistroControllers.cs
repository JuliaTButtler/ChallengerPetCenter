using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RegistrosController : ControllerBase
    {
        private readonly IRegistroService _registroService;

        public RegistrosController(IRegistroService registroService)
        {
            _registroService = registroService;
        }

        /// <summary>
        /// Lista todos os registros cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistros()
        {
            var registros = await _registroService.GetAllAsync();
            return Ok(registros);
        }

        /// <summary>
        /// Busca um registro pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RegistroResponseDto>> GetRegistroById(int id)
        {
            var result = await _registroService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca registros de uma entrada do diário.
        /// </summary>
        [HttpGet("diario/{diarioEntradaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistrosByDiario(int diarioEntradaId)
        {
            var registros = (await _registroService.GetByDiarioAsync(diarioEntradaId)).ToList();
            if (!registros.Any())
                return NotFound("Nenhum registro encontrado.");

            return Ok(registros);
        }

        /// <summary>
        /// Busca registros por tipo.
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistrosByTipo(string tipo)
        {
            var registros = (await _registroService.GetByTipoAsync(tipo)).ToList();
            if (!registros.Any())
                return NotFound("Nenhum registro encontrado.");

            return Ok(registros);
        }

        /// <summary>
        /// Busca registros por subtipo.
        /// </summary>
        [HttpGet("subtipo/{subtipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistrosBySubtipo(string subtipo)
        {
            var registros = (await _registroService.GetBySubtipoAsync(subtipo)).ToList();
            if (!registros.Any())
                return NotFound("Nenhum registro encontrado.");

            return Ok(registros);
        }

        /// <summary>
        /// Cria um novo registro.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegistroResponseDto>> CreateRegistro(CreateRegistroDto dto)
        {
            var result = await _registroService.CreateAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetRegistroById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza um registro existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRegistro(int id, UpdateRegistroDto dto)
        {
            var result = await _registroService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um registro.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRegistro(int id)
        {
            var result = await _registroService.DeleteAsync(id);
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
