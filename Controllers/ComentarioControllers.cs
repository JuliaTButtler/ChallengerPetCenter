using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ComentariosController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;

        public ComentariosController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        /// <summary>
        /// Lista todos os comentários cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentarios()
        {
            var result = await _comentarioService.GetAllAsync();
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca um comentário pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComentarioResponseDto>> GetComentarioById(int id)
        {
            var result = await _comentarioService.GetByIdAsync(id);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca comentários de uma entrada do diário.
        /// </summary>
        [HttpGet("diario/{diarioEntradaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosByDiario(int diarioEntradaId)
        {
            var result = await _comentarioService.GetByDiarioAsync(diarioEntradaId);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca comentários de um usuário.
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosByUser(int userId)
        {
            var result = await _comentarioService.GetByUserAsync(userId);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca comentários pelo texto.
        /// </summary>
        [HttpGet("texto/{texto}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosByTexto(string texto)
        {
            var result = await _comentarioService.GetByTextoAsync(texto);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Cria um novo comentário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComentarioResponseDto>> CreateComentario(CreateComentarioDto dto)
        {
            var result = await _comentarioService.CreateAsync(dto);
            return this.ToCreatedResult(result, nameof(GetComentarioById), new { id = result.Data?.Id });
        }

        /// <summary>
        /// Atualiza um comentário existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComentario(int id, UpdateComentarioDto dto)
        {
            var result = await _comentarioService.UpdateAsync(id, dto);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Remove um comentário.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var result = await _comentarioService.DeleteAsync(id);
            return this.ToActionResult(result);
        }
    }
}
