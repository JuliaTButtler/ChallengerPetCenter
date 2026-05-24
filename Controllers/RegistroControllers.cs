using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RegistrosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegistrosController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: api/registros
        // Lista todos os registros
        // =====================================================

        /// <summary>
        /// Lista todos os registros cadastrados.
        /// </summary>
        /// <returns>Lista de registros.</returns>
        /// <response code="200">Registros encontrados com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistros()
        {
            var registros = await _context.Registros
                .ToListAsync();

            return Ok(registros);
        }

        // =====================================================
        // GET: api/registros/1
        // Buscar registro por ID
        // =====================================================

        /// <summary>
        /// Busca um registro pelo ID.
        /// </summary>
        /// <param name="id">ID do registro.</param>
        /// <returns>Registro encontrado.</returns>
        /// <response code="200">Registro encontrado.</response>
        /// <response code="404">Registro não encontrado.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Registro>> GetRegistroById(int id)
        {
            var registro = await _context.Registros
                .FindAsync(id);

            if (registro == null)
            {
                return NotFound("Registro não encontrado.");
            }

            return Ok(registro);
        }

        // =====================================================
        // GET: api/registros/diario/1
        // Buscar registros por entrada do diário
        // =====================================================

        /// <summary>
        /// Busca registros de uma entrada do diário.
        /// </summary>
        /// <param name="diarioEntradaId">ID da entrada do diário.</param>
        /// <returns>Lista de registros.</returns>
        /// <response code="200">Registros encontrados.</response>
        /// <response code="404">Nenhum registro encontrado.</response>

        [HttpGet("diario/{diarioEntradaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistrosByDiario(int diarioEntradaId)
        {
            var registros = await _context.Registros
                .Where(r => r.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

            if (!registros.Any())
            {
                return NotFound("Nenhum registro encontrado.");
            }

            return Ok(registros);
        }

        // =====================================================
        // GET: api/registros/tipo/alimentacao
        // Buscar registros por tipo
        // =====================================================

        /// <summary>
        /// Busca registros por tipo.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <returns>Lista de registros.</returns>
        /// <response code="200">Registros encontrados.</response>
        /// <response code="404">Nenhum registro encontrado.</response>

        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistrosByTipo(string tipo)
        {
            var registros = await _context.Registros
                .Where(r =>
                    r.Tipo.ToUpper() == tipo.ToUpper())
                .ToListAsync();

            if (!registros.Any())
            {
                return NotFound("Nenhum registro encontrado.");
            }

            return Ok(registros);
        }

        // =====================================================
        // GET: api/registros/subtipo/racao
        // Buscar registros por subtipo
        // =====================================================

        /// <summary>
        /// Busca registros por subtipo.
        /// </summary>
        /// <param name="subtipo">Subtipo do registro.</param>
        /// <returns>Lista de registros.</returns>
        /// <response code="200">Registros encontrados.</response>
        /// <response code="404">Nenhum registro encontrado.</response>

        [HttpGet("subtipo/{subtipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Registro>>> GetRegistrosBySubtipo(string subtipo)
        {
            var registros = await _context.Registros
                .Where(r =>
                    r.Subtipo != null &&
                    r.Subtipo.ToUpper() == subtipo.ToUpper())
                .ToListAsync();

            if (!registros.Any())
            {
                return NotFound("Nenhum registro encontrado.");
            }

            return Ok(registros);
        }

        // =====================================================
        // POST: api/registros
        // Criar registro
        // =====================================================

        /// <summary>
        /// Cria um novo registro.
        /// </summary>
        /// <param name="registro">Dados do registro.</param>
        /// <returns>Registro criado.</returns>
        /// <response code="201">Registro criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Registro>> CreateRegistro(Registro registro)
        {
            registro.DiarioEntrada = null;

            if (string.IsNullOrWhiteSpace(registro.Tipo))
            {
                return BadRequest("Tipo é obrigatório.");
            }

            // Verificar se entrada do diário existe

            var diarioExists = await _context.DiarioEntradas
                .CountAsync(d => d.Id == registro.DiarioEntradaId);

            if (diarioExists == 0)
            {
                return BadRequest("Entrada do diário não encontrada.");
            }

            _context.Registros.Add(registro);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRegistroById),
                new { id = registro.Id },
                registro
            );
        }

        // =====================================================
        // PUT: api/registros/1
        // Atualizar registro
        // =====================================================

        /// <summary>
        /// Atualiza um registro existente.
        /// </summary>
        /// <param name="id">ID do registro.</param>
        /// <param name="updatedRegistro">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Registro atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Registro não encontrado.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRegistro(int id, Registro updatedRegistro)
        {
            if (id != updatedRegistro.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingRegistro = await _context.Registros
                .FindAsync(id);

            if (existingRegistro == null)
            {
                return NotFound("Registro não encontrado.");
            }

            existingRegistro.Tipo = updatedRegistro.Tipo;
            existingRegistro.Subtipo = updatedRegistro.Subtipo;
            existingRegistro.Valor = updatedRegistro.Valor;
            existingRegistro.Unidade = updatedRegistro.Unidade;
            existingRegistro.Nota = updatedRegistro.Nota;
            existingRegistro.Horario = updatedRegistro.Horario;
            existingRegistro.AtualizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // DELETE: api/registros/1
        // Remover registro
        // =====================================================

        /// <summary>
        /// Remove um registro.
        /// </summary>
        /// <param name="id">ID do registro.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Registro removido com sucesso.</response>
        /// <response code="404">Registro não encontrado.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRegistro(int id)
        {
            var registro = await _context.Registros
                .FindAsync(id);

            if (registro == null)
            {
                return NotFound("Registro não encontrado.");
            }

            _context.Registros.Remove(registro);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}