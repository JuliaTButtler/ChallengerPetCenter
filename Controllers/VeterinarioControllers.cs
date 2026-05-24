using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VeterinariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeterinariosController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: api/veterinarios
        // Lista todos os veterinários
        // =====================================================

        /// <summary>
        /// Lista todos os veterinários cadastrados.
        /// </summary>
        /// <returns>Lista de veterinários.</returns>
        /// <response code="200">Veterinários encontrados com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Veterinario>>> GetVeterinarios()
        {
            var veterinarios = await _context.Veterinarios
                .ToListAsync();

            return Ok(veterinarios);
        }

        // =====================================================
        // GET: api/veterinarios/1
        // Buscar veterinário por ID
        // =====================================================

        /// <summary>
        /// Busca um veterinário pelo ID.
        /// </summary>
        /// <param name="id">ID do veterinário.</param>
        /// <returns>Veterinário encontrado.</returns>
        /// <response code="200">Veterinário encontrado.</response>
        /// <response code="404">Veterinário não encontrado.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Veterinario>> GetVeterinarioById(int id)
        {
            var veterinario = await _context.Veterinarios
                .FindAsync(id);

            if (veterinario == null)
            {
                return NotFound("Veterinário não encontrado.");
            }

            return Ok(veterinario);
        }

        // =====================================================
        // GET: api/veterinarios/crmv/12345
        // Buscar veterinário por CRMV
        // =====================================================

        /// <summary>
        /// Busca um veterinário pelo CRMV.
        /// </summary>
        /// <param name="crmv">CRMV do veterinário.</param>
        /// <returns>Veterinário encontrado.</returns>
        /// <response code="200">Veterinário encontrado.</response>
        /// <response code="404">Veterinário não encontrado.</response>

        [HttpGet("crmv/{crmv}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Veterinario>> GetVeterinarioByCrmv(string crmv)
        {
            var veterinario = await _context.Veterinarios
                .FirstOrDefaultAsync(v => v.Crmv == crmv);

            if (veterinario == null)
            {
                return NotFound("Veterinário não encontrado.");
            }

            return Ok(veterinario);
        }

        // =====================================================
        // GET: api/veterinarios/especialidade/cardiologia
        // Buscar veterinários por especialidade
        // =====================================================

        /// <summary>
        /// Busca veterinários por especialidade.
        /// </summary>
        /// <param name="especialidade">Especialidade do veterinário.</param>
        /// <returns>Lista de veterinários.</returns>
        /// <response code="200">Veterinários encontrados.</response>
        /// <response code="404">Nenhum veterinário encontrado.</response>

        [HttpGet("especialidade/{especialidade}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Veterinario>>> GetVeterinariosByEspecialidade(string especialidade)
        {
            especialidade = especialidade.ToUpper();

            var veterinarios = await _context.Veterinarios
                .Where(v =>
                    v.Especialidade != null &&
                    v.Especialidade.ToUpper() == especialidade)
                .ToListAsync();

            if (!veterinarios.Any())
            {
                return NotFound("Nenhum veterinário encontrado.");
            }

            return Ok(veterinarios);
        }

        // =====================================================
        // GET: api/veterinarios/user/1
        // Buscar veterinário pelo UserId
        // =====================================================

        /// <summary>
        /// Busca um veterinário pelo ID do usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>Veterinário encontrado.</returns>
        /// <response code="200">Veterinário encontrado.</response>
        /// <response code="404">Veterinário não encontrado.</response>

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Veterinario>> GetVeterinarioByUserId(int userId)
        {
            var veterinario = await _context.Veterinarios
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (veterinario == null)
            {
                return NotFound("Veterinário não encontrado.");
            }

            return Ok(veterinario);
        }

        // =====================================================
        // POST: api/veterinarios
        // Criar veterinário
        // =====================================================

        /// <summary>
        /// Cria um novo veterinário.
        /// </summary>
        /// <param name="veterinario">Dados do veterinário.</param>
        /// <returns>Veterinário criado.</returns>
        /// <response code="201">Veterinário criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Veterinario>> CreateVeterinario(Veterinario veterinario)
        {
            if (string.IsNullOrWhiteSpace(veterinario.Crmv))
            {
                return BadRequest("CRMV é obrigatório.");
            }

            // Verificar CRMV duplicado

            var crmvCount = await _context.Veterinarios
                .CountAsync(v => v.Crmv == veterinario.Crmv);

            if (crmvCount > 0)
            {
                return BadRequest("CRMV já cadastrado.");
            }

            // Verificar se User existe

            var userCount = await _context.Users
                .CountAsync(u => u.Id == veterinario.UserId);

            if (userCount == 0)
            {
                return BadRequest("Usuário não encontrado.");
            }

            _context.Veterinarios.Add(veterinario);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVeterinarioById),
                new { id = veterinario.Id },
                veterinario
            );
        }

        // =====================================================
        // PUT: api/veterinarios/1
        // Atualizar veterinário
        // =====================================================

        /// <summary>
        /// Atualiza um veterinário existente.
        /// </summary>
        /// <param name="id">ID do veterinário.</param>
        /// <param name="updatedVeterinario">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Veterinário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Veterinário não encontrado.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVeterinario(int id, Veterinario updatedVeterinario)
        {
            if (id != updatedVeterinario.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingVeterinario = await _context.Veterinarios
                .FindAsync(id);

            if (existingVeterinario == null)
            {
                return NotFound("Veterinário não encontrado.");
            }

            existingVeterinario.Crmv = updatedVeterinario.Crmv;
            existingVeterinario.Especialidade = updatedVeterinario.Especialidade;
            existingVeterinario.Descricao = updatedVeterinario.Descricao;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // DELETE: api/veterinarios/1
        // Remover veterinário
        // =====================================================

        /// <summary>
        /// Remove um veterinário.
        /// </summary>
        /// <param name="id">ID do veterinário.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Veterinário removido com sucesso.</response>
        /// <response code="404">Veterinário não encontrado.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVeterinario(int id)
        {
            var veterinario = await _context.Veterinarios
                .FindAsync(id);

            if (veterinario == null)
            {
                return NotFound("Veterinário não encontrado.");
            }

            _context.Veterinarios.Remove(veterinario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}