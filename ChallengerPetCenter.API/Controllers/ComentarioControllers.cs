using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ComentariosController : ApiControllerBase
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
            var comentarios = await _comentarioService.GetAllAsync();
            return Ok(comentarios);
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
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca comentários de uma entrada do diário.
        /// </summary>
        [HttpGet("diario/{diarioEntradaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosByDiario(int diarioEntradaId)
        {
            var comentarios = (await _comentarioService.GetByDiarioAsync(diarioEntradaId)).ToList();
            if (!comentarios.Any())
                return ProblemNotFound("Nenhum comentário encontrado.");

            return Ok(comentarios);
        }

        /// <summary>
        /// Busca comentários de um usuário.
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosByUser(int userId)
        {
            var comentarios = (await _comentarioService.GetByUserAsync(userId)).ToList();
            if (!comentarios.Any())
                return ProblemNotFound("Nenhum comentário encontrado.");

            return Ok(comentarios);
        }

        /// <summary>
        /// Busca comentários pelo texto.
        /// </summary>
        [HttpGet("texto/{texto}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosByTexto(string texto)
        {
            var comentarios = (await _comentarioService.GetByTextoAsync(texto)).ToList();
            if (!comentarios.Any())
                return ProblemNotFound("Nenhum comentário encontrado.");

            return Ok(comentarios);
        }

        /// <summary>
        /// Cria um novo comentário.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComentarioResponseDto>> CreateComentario(CreateComentarioDto dto)
        {
            var result = await _comentarioService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            return CreatedAtAction(
                nameof(GetComentarioById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza um comentário existente.
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComentario(int id, UpdateComentarioDto dto)
        {
            var result = await _comentarioService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um comentário.
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var result = await _comentarioService.DeleteAsync(id);
            return ToActionResult(result);
        }
    }
}
