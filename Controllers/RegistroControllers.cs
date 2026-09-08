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
            var result = await _registroService.GetAllAsync();
            return this.ToActionResult(result);
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
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca registros de uma entrada do diário.
        /// </summary>
        [HttpGet("diario/{diarioEntradaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistrosByDiario(int diarioEntradaId)
        {
            var result = await _registroService.GetByDiarioAsync(diarioEntradaId);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca registros por tipo.
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistrosByTipo(string tipo)
        {
            var result = await _registroService.GetByTipoAsync(tipo);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca registros por subtipo.
        /// </summary>
        [HttpGet("subtipo/{subtipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistroResponseDto>>> GetRegistrosBySubtipo(string subtipo)
        {
            var result = await _registroService.GetBySubtipoAsync(subtipo);
            return this.ToActionResult(result);
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
            return this.ToCreatedResult(result, nameof(GetRegistroById), new { id = result.Data?.Id });
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
            return this.ToActionResult(result);
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
            return this.ToActionResult(result);
        }
    }
}
