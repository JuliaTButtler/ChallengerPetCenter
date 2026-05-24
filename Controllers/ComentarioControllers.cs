using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComentariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/comentarios
        // Lista todos os comentários

        /// <summary>
        /// Lista todos os comentários cadastrados.
        /// </summary>
        /// <returns>Lista de comentários.</returns>
        /// <response code="200">Comentários encontrados com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            var comentarios = await _context.Comentarios
                .ToListAsync();

            return Ok(comentarios);
        }

        // GET: api/comentarios/1
        // Buscar comentário por ID

        /// <summary>
        /// Busca um comentário pelo ID.
        /// </summary>
        /// <param name="id">ID do comentário.</param>
        /// <returns>Comentário encontrado.</returns>
        /// <response code="200">Comentário encontrado.</response>
        /// <response code="404">Comentário não encontrado.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comentario>> GetComentarioById(int id)
        {
            var comentario = await _context.Comentarios
                .FindAsync(id);

            if (comentario == null)
            {
                return NotFound("Comentário não encontrado.");
            }

            return Ok(comentario);
        }

        // GET: api/comentarios/diario/1
        // Buscar comentários de uma entrada do diário

        /// <summary>
        /// Busca comentários de uma entrada do diário.
        /// </summary>
        /// <param name="diarioEntradaId">ID da entrada do diário.</param>
        /// <returns>Lista de comentários.</returns>
        /// <response code="200">Comentários encontrados.</response>
        /// <response code="404">Nenhum comentário encontrado.</response>

        [HttpGet("diario/{diarioEntradaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentariosByDiario(int diarioEntradaId)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

            if (!comentarios.Any())
            {
                return NotFound("Nenhum comentário encontrado.");
            }

            return Ok(comentarios);
        }

        // GET: api/comentarios/user/1
        // Buscar comentários de um usuário

        /// <summary>
        /// Busca comentários de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>Lista de comentários.</returns>
        /// <response code="200">Comentários encontrados.</response>
        /// <response code="404">Nenhum comentário encontrado.</response>

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentariosByUser(int userId)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!comentarios.Any())
            {
                return NotFound("Nenhum comentário encontrado.");
            }

            return Ok(comentarios);
        }

        // GET: api/comentarios/texto/teste
        // Buscar comentários por texto

        /// <summary>
        /// Busca comentários pelo texto.
        /// </summary>
        /// <param name="texto">Texto do comentário.</param>
        /// <returns>Lista de comentários.</returns>
        /// <response code="200">Comentários encontrados.</response>
        /// <response code="404">Nenhum comentário encontrado.</response>

        [HttpGet("texto/{texto}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentariosByTexto(string texto)
        {
            var comentarios = await _context.Comentarios
                .Where(c =>
                    c.TextoComentario.ToUpper()
                    .Contains(texto.ToUpper()))
                .ToListAsync();

            if (!comentarios.Any())
            {
                return NotFound("Nenhum comentário encontrado.");
            }

            return Ok(comentarios);
        }

        // POST: api/comentarios
        // Criar comentário

        /// <summary>
        /// Cria um novo comentário.
        /// </summary>
        /// <param name="comentario">Dados do comentário.</param>
        /// <returns>Comentário criado.</returns>
        /// <response code="201">Comentário criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Comentario>> CreateComentario(Comentario comentario)
        {
            comentario.User = null!;
            comentario.DiarioEntrada = null;

            if (string.IsNullOrWhiteSpace(comentario.TextoComentario))
            {
                return BadRequest("Texto do comentário é obrigatório.");
            }

            // Verificar se entrada do diário existe

            var diarioCount = await _context.DiarioEntradas
                .CountAsync(d => d.Id == comentario.DiarioEntradaId);

            if (diarioCount == 0)
            {
                return BadRequest("Entrada do diário não encontrada.");
            }

            // Verificar se usuário existe

            var userCount = await _context.Users
                .CountAsync(u => u.Id == comentario.UserId);

            if (userCount == 0)
            {
                return BadRequest("Usuário não encontrado.");
            }

            _context.Comentarios.Add(comentario);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetComentarioById),
                new { id = comentario.Id },
                comentario
            );
        }

        // PUT: api/comentarios/1
        // Atualizar comentário

        /// <summary>
        /// Atualiza um comentário existente.
        /// </summary>
        /// <param name="id">ID do comentário.</param>
        /// <param name="updatedComentario">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Comentário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Comentário não encontrado.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComentario(int id, Comentario updatedComentario)
        {
            if (id != updatedComentario.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingComentario = await _context.Comentarios
                .FindAsync(id);

            if (existingComentario == null)
            {
                return NotFound("Comentário não encontrado.");
            }

            existingComentario.TextoComentario = updatedComentario.TextoComentario;
            existingComentario.EditadoEm = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/comentarios/1
        // Remover comentário

        /// <summary>
        /// Remove um comentário.
        /// </summary>
        /// <param name="id">ID do comentário.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Comentário removido com sucesso.</response>
        /// <response code="404">Comentário não encontrado.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentarios
                .FindAsync(id);

            if (comentario == null)
            {
                return NotFound("Comentário não encontrado.");
            }

            _context.Comentarios.Remove(comentario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}