using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiarioEntradasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiarioEntradasController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: api/diarioentradas
        // Lista todas as entradas do diário
        // =====================================================

        /// <summary>
        /// Lista todas as entradas do diário.
        /// </summary>
        /// <returns>Lista de entradas.</returns>
        /// <response code="200">Entradas encontradas com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DiarioEntrada>>> GetDiarioEntradas()
        {
            var entradas = await _context.DiarioEntradas
                .ToListAsync();

            return Ok(entradas);
        }

        // =====================================================
        // GET: api/diarioentradas/1
        // Buscar entrada por ID
        // =====================================================

        /// <summary>
        /// Busca uma entrada pelo ID.
        /// </summary>
        /// <param name="id">ID da entrada.</param>
        /// <returns>Entrada encontrada.</returns>
        /// <response code="200">Entrada encontrada.</response>
        /// <response code="404">Entrada não encontrada.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DiarioEntrada>> GetDiarioEntradaById(int id)
        {
            var entrada = await _context.DiarioEntradas
                .FindAsync(id);

            if (entrada == null)
            {
                return NotFound("Entrada não encontrada.");
            }

            return Ok(entrada);
        }

        // =====================================================
        // GET: api/diarioentradas/pet/1
        // Buscar entradas por pet
        // =====================================================

        /// <summary>
        /// Busca entradas de um pet.
        /// </summary>
        /// <param name="petId">ID do pet.</param>
        /// <returns>Lista de entradas.</returns>
        /// <response code="200">Entradas encontradas.</response>
        /// <response code="404">Nenhuma entrada encontrada.</response>

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DiarioEntrada>>> GetEntradasByPet(int petId)
        {
            var entradas = await _context.DiarioEntradas
                .Where(d => d.PetId == petId)
                .ToListAsync();

            if (!entradas.Any())
            {
                return NotFound("Nenhuma entrada encontrada.");
            }

            return Ok(entradas);
        }

        // =====================================================
        // GET: api/diarioentradas/status/ATIVO
        // Buscar entradas por status
        // =====================================================

        /// <summary>
        /// Busca entradas por status.
        /// </summary>
        /// <param name="status">Status da entrada.</param>
        /// <returns>Lista de entradas.</returns>
        /// <response code="200">Entradas encontradas.</response>
        /// <response code="404">Nenhuma entrada encontrada.</response>

        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DiarioEntrada>>> GetEntradasByStatus(string status)
        {
            var entradas = await _context.DiarioEntradas
                .Where(d =>
                    d.Status != null &&
                    d.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

            if (!entradas.Any())
            {
                return NotFound("Nenhuma entrada encontrada.");
            }

            return Ok(entradas);
        }

        // =====================================================
        // GET: api/diarioentradas/humor/FELIZ
        // Buscar entradas por humor
        // =====================================================

        /// <summary>
        /// Busca entradas por humor geral.
        /// </summary>
        /// <param name="humor">Humor da entrada.</param>
        /// <returns>Lista de entradas.</returns>
        /// <response code="200">Entradas encontradas.</response>
        /// <response code="404">Nenhuma entrada encontrada.</response>

        [HttpGet("humor/{humor}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DiarioEntrada>>> GetEntradasByHumor(string humor)
        {
            var entradas = await _context.DiarioEntradas
                .Where(d =>
                    d.HumorGeral != null &&
                    d.HumorGeral.ToUpper() == humor.ToUpper())
                .ToListAsync();

            if (!entradas.Any())
            {
                return NotFound("Nenhuma entrada encontrada.");
            }

            return Ok(entradas);
        }

        // =====================================================
        // POST: api/diarioentradas
        // Criar entrada
        // =====================================================

        /// <summary>
        /// Cria uma nova entrada no diário.
        /// </summary>
        /// <param name="entrada">Dados da entrada.</param>
        /// <returns>Entrada criada.</returns>
        /// <response code="201">Entrada criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DiarioEntrada>> CreateDiarioEntrada(DiarioEntrada entrada)
        {
            var petExists = await _context.Pets
                .CountAsync(p => p.Id == entrada.PetId);

            if (petExists == 0)
            {
                return BadRequest("Pet não encontrado.");
            }

            var entradaExiste = await _context.DiarioEntradas
                .CountAsync(d =>
                    d.PetId == entrada.PetId &&
                    d.Data.Date == entrada.Data.Date);

            if (entradaExiste > 0)
            {
                return BadRequest("Já existe uma entrada para este pet nesta data.");
            }

            _context.DiarioEntradas.Add(entrada);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDiarioEntradaById),
                new { id = entrada.Id },
                entrada
            );
        }

        // =====================================================
        // PUT: api/diarioentradas/1
        // Atualizar entrada
        // =====================================================

        /// <summary>
        /// Atualiza uma entrada do diário.
        /// </summary>
        /// <param name="id">ID da entrada.</param>
        /// <param name="updatedEntrada">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Entrada atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Entrada não encontrada.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDiarioEntrada(int id, DiarioEntrada updatedEntrada)
        {
            if (id != updatedEntrada.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingEntrada = await _context.DiarioEntradas
                .FindAsync(id);

            if (existingEntrada == null)
            {
                return NotFound("Entrada não encontrada.");
            }

            existingEntrada.Data = updatedEntrada.Data;
            existingEntrada.Resumo = updatedEntrada.Resumo;
            existingEntrada.HumorGeral = updatedEntrada.HumorGeral;
            existingEntrada.Status = updatedEntrada.Status;
            existingEntrada.AtualizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // DELETE: api/diarioentradas/1
        // Remover entrada
        // =====================================================

        /// <summary>
        /// Remove uma entrada do diário.
        /// </summary>
        /// <param name="id">ID da entrada.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Entrada removida com sucesso.</response>
        /// <response code="404">Entrada não encontrada.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDiarioEntrada(int id)
        {
            var entrada = await _context.DiarioEntradas
                .FindAsync(id);

            if (entrada == null)
            {
                return NotFound("Entrada não encontrada.");
            }

            _context.DiarioEntradas.Remove(entrada);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}