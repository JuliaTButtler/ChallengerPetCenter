using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetVeterinariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetVeterinariosController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: api/petveterinarios
        // Lista todos os vínculos
        // =====================================================

        /// <summary>
        /// Lista todos os vínculos entre pets e veterinários.
        /// </summary>
        /// <returns>Lista de vínculos.</returns>
        /// <response code="200">Vínculos encontrados com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PetVeterinario>>> GetPetVeterinarios()
        {
            var petVeterinarios = await _context.PetVeterinarios
                .ToListAsync();

            return Ok(petVeterinarios);
        }

        // =====================================================
        // GET: api/petveterinarios/1
        // Buscar vínculo por ID
        // =====================================================

        /// <summary>
        /// Busca um vínculo pelo ID.
        /// </summary>
        /// <param name="id">ID do vínculo.</param>
        /// <returns>Vínculo encontrado.</returns>
        /// <response code="200">Vínculo encontrado.</response>
        /// <response code="404">Vínculo não encontrado.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetVeterinario>> GetPetVeterinarioById(int id)
        {
            var petVeterinario = await _context.PetVeterinarios
                .FindAsync(id);

            if (petVeterinario == null)
            {
                return NotFound("Vínculo não encontrado.");
            }

            return Ok(petVeterinario);
        }

        // =====================================================
        // GET: api/petveterinarios/pet/1
        // Buscar vínculos por pet
        // =====================================================

        /// <summary>
        /// Busca vínculos de um pet.
        /// </summary>
        /// <param name="petId">ID do pet.</param>
        /// <returns>Lista de vínculos.</returns>
        /// <response code="200">Vínculos encontrados.</response>
        /// <response code="404">Nenhum vínculo encontrado.</response>

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinario>>> GetByPet(int petId)
        {
            var petVeterinarios = await _context.PetVeterinarios
                .Where(pv => pv.PetId == petId)
                .ToListAsync();

            if (!petVeterinarios.Any())
            {
                return NotFound("Nenhum vínculo encontrado.");
            }

            return Ok(petVeterinarios);
        }

        // =====================================================
        // GET: api/petveterinarios/veterinario/1
        // Buscar vínculos por veterinário
        // =====================================================

        /// <summary>
        /// Busca vínculos de um veterinário.
        /// </summary>
        /// <param name="veterinarioId">ID do veterinário.</param>
        /// <returns>Lista de vínculos.</returns>
        /// <response code="200">Vínculos encontrados.</response>
        /// <response code="404">Nenhum vínculo encontrado.</response>

        [HttpGet("veterinario/{veterinarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinario>>> GetByVeterinario(int veterinarioId)
        {
            var petVeterinarios = await _context.PetVeterinarios
                .Where(pv => pv.VeterinarioId == veterinarioId)
                .ToListAsync();

            if (!petVeterinarios.Any())
            {
                return NotFound("Nenhum vínculo encontrado.");
            }

            return Ok(petVeterinarios);
        }

        // =====================================================
        // GET: api/petveterinarios/ativos
        // Buscar vínculos ativos
        // =====================================================

        /// <summary>
        /// Busca vínculos ativos.
        /// </summary>
        /// <returns>Lista de vínculos ativos.</returns>
        /// <response code="200">Vínculos encontrados.</response>
        /// <response code="404">Nenhum vínculo encontrado.</response>

        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinario>>> GetAtivos()
        {
            var petVeterinarios = await _context.PetVeterinarios
                .Where(pv => pv.Ativo == 1)
                .ToListAsync();

            if (!petVeterinarios.Any())
            {
                return NotFound("Nenhum vínculo ativo encontrado.");
            }

            return Ok(petVeterinarios);
        }

        // =====================================================
        // POST: api/petveterinarios
        // Criar vínculo
        // =====================================================

        /// <summary>
        /// Cria um novo vínculo entre pet e veterinário.
        /// </summary>
        /// <param name="petVeterinario">Dados do vínculo.</param>
        /// <returns>Vínculo criado.</returns>
        /// <response code="201">Vínculo criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PetVeterinario>> CreatePetVeterinario(PetVeterinario petVeterinario)
        {
            petVeterinario.Pet = null;
            petVeterinario.Veterinario = null;

            // Verificar se pet existe

            var petCount = await _context.Pets
                .CountAsync(p => p.Id == petVeterinario.PetId);

            if (petCount == 0)
            {
                return BadRequest("Pet não encontrado.");
            }

            // Verificar se veterinário existe

            var veterinarioCount = await _context.Veterinarios
                .CountAsync(v => v.Id == petVeterinario.VeterinarioId);

            if (veterinarioCount == 0)
            {
                return BadRequest("Veterinário não encontrado.");
            }

            // Verificar vínculo duplicado

            var vinculoCount = await _context.PetVeterinarios
                .CountAsync(pv =>
                    pv.PetId == petVeterinario.PetId &&
                    pv.VeterinarioId == petVeterinario.VeterinarioId &&
                    pv.Ativo == 1);

            if (vinculoCount > 0)
            {
                return BadRequest("Já existe um vínculo ativo entre este pet e veterinário.");
            }

            _context.PetVeterinarios.Add(petVeterinario);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPetVeterinarioById),
                new { id = petVeterinario.Id },
                petVeterinario
            );
        }

        // =====================================================
        // PUT: api/petveterinarios/1
        // Atualizar vínculo
        // =====================================================

        /// <summary>
        /// Atualiza um vínculo existente.
        /// </summary>
        /// <param name="id">ID do vínculo.</param>
        /// <param name="updatedPetVeterinario">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Vínculo atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Vínculo não encontrado.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePetVeterinario(
            int id,
            PetVeterinario updatedPetVeterinario)
        {
            if (id != updatedPetVeterinario.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingPetVeterinario = await _context.PetVeterinarios
                .FindAsync(id);

            if (existingPetVeterinario == null)
            {
                return NotFound("Vínculo não encontrado.");
            }

            existingPetVeterinario.Ativo = updatedPetVeterinario.Ativo;
            existingPetVeterinario.Observacoes = updatedPetVeterinario.Observacoes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // DELETE: api/petveterinarios/1
        // Remover vínculo
        // =====================================================

        /// <summary>
        /// Remove um vínculo.
        /// </summary>
        /// <param name="id">ID do vínculo.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Vínculo removido com sucesso.</response>
        /// <response code="404">Vínculo não encontrado.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePetVeterinario(int id)
        {
            var petVeterinario = await _context.PetVeterinarios
                .FindAsync(id);

            if (petVeterinario == null)
            {
                return NotFound("Vínculo não encontrado.");
            }

            _context.PetVeterinarios.Remove(petVeterinario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}