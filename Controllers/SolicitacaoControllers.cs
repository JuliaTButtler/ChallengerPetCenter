using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SolicitacoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SolicitacoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/solicitacoes
        // Lista todas as solicitações

        /// <summary>
        /// Lista todas as solicitações cadastradas.
        /// </summary>
        /// <returns>Lista de solicitações.</returns>
        /// <response code="200">Solicitações encontradas com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetSolicitacoes()
        {
            var solicitacoes = await _context.Solicitacoes
                .ToListAsync();

            return Ok(solicitacoes);
        }

        // GET: api/solicitacoes/1
        // Buscar solicitação por ID

        /// <summary>
        /// Busca uma solicitação pelo ID.
        /// </summary>
        /// <param name="id">ID da solicitação.</param>
        /// <returns>Solicitação encontrada.</returns>
        /// <response code="200">Solicitação encontrada.</response>
        /// <response code="404">Solicitação não encontrada.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Solicitacao>> GetSolicitacaoById(int id)
        {
            var solicitacao = await _context.Solicitacoes
                .FindAsync(id);

            if (solicitacao == null)
            {
                return NotFound("Solicitação não encontrada.");
            }

            return Ok(solicitacao);
        }

        // GET: api/solicitacoes/pet/1
        // Buscar solicitações de um pet

        /// <summary>
        /// Busca solicitações de um pet.
        /// </summary>
        /// <param name="petId">ID do pet.</param>
        /// <returns>Lista de solicitações.</returns>
        /// <response code="200">Solicitações encontradas.</response>
        /// <response code="404">Nenhuma solicitação encontrada.</response>

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetSolicitacoesByPet(int petId)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.PetId == petId)
                .ToListAsync();

            if (!solicitacoes.Any())
            {
                return NotFound("Nenhuma solicitação encontrada.");
            }

            return Ok(solicitacoes);
        }

        // GET: api/solicitacoes/tutor/1
        // Buscar solicitações de um tutor

        /// <summary>
        /// Busca solicitações de um tutor.
        /// </summary>
        /// <param name="tutorId">ID do tutor.</param>
        /// <returns>Lista de solicitações.</returns>
        /// <response code="200">Solicitações encontradas.</response>
        /// <response code="404">Nenhuma solicitação encontrada.</response>

        [HttpGet("tutor/{tutorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetSolicitacoesByTutor(int tutorId)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.TutorId == tutorId)
                .ToListAsync();

            if (!solicitacoes.Any())
            {
                return NotFound("Nenhuma solicitação encontrada.");
            }

            return Ok(solicitacoes);
        }

        // GET: api/solicitacoes/veterinario/1
        // Buscar solicitações de um veterinário

        /// <summary>
        /// Busca solicitações de um veterinário.
        /// </summary>
        /// <param name="veterinarioId">ID do veterinário.</param>
        /// <returns>Lista de solicitações.</returns>
        /// <response code="200">Solicitações encontradas.</response>
        /// <response code="404">Nenhuma solicitação encontrada.</response>

        [HttpGet("veterinario/{veterinarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetSolicitacoesByVeterinario(int veterinarioId)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.VeterinarioId == veterinarioId)
                .ToListAsync();

            if (!solicitacoes.Any())
            {
                return NotFound("Nenhuma solicitação encontrada.");
            }

            return Ok(solicitacoes);
        }

        // GET: api/solicitacoes/status/PENDENTE
        // Buscar solicitações por status

        /// <summary>
        /// Busca solicitações por status.
        /// </summary>
        /// <param name="status">Status da solicitação.</param>
        /// <returns>Lista de solicitações.</returns>
        /// <response code="200">Solicitações encontradas.</response>
        /// <response code="404">Nenhuma solicitação encontrada.</response>

        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetSolicitacoesByStatus(string status)
        {
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

            if (!solicitacoes.Any())
            {
                return NotFound("Nenhuma solicitação encontrada.");
            }

            return Ok(solicitacoes);
        }

        // POST: api/solicitacoes
        // Criar solicitação

        /// <summary>
        /// Cria uma nova solicitação.
        /// </summary>
        /// <param name="solicitacao">Dados da solicitação.</param>
        /// <returns>Solicitação criada.</returns>
        /// <response code="201">Solicitação criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Solicitacao>> CreateSolicitacao(Solicitacao solicitacao)
        {
            solicitacao.Pet = null;
            solicitacao.Tutor = null;
            solicitacao.Veterinario = null;

            if (string.IsNullOrWhiteSpace(solicitacao.Status))
            {
                return BadRequest("Status é obrigatório.");
            }

            // Verificar se pet existe

            var petCount = await _context.Pets
                .CountAsync(p => p.Id == solicitacao.PetId);

            if (petCount == 0)
            {
                return BadRequest("Pet não encontrado.");
            }

            // Verificar se tutor existe

            var tutorCount = await _context.Users
                .CountAsync(u => u.Id == solicitacao.TutorId);

            if (tutorCount == 0)
            {
                return BadRequest("Tutor não encontrado.");
            }

            // Verificar se veterinário existe

            var veterinarioCount = await _context.Veterinarios
                .CountAsync(v => v.Id == solicitacao.VeterinarioId);

            if (veterinarioCount == 0)
            {
                return BadRequest("Veterinário não encontrado.");
            }

            _context.Solicitacoes.Add(solicitacao);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSolicitacaoById),
                new { id = solicitacao.Id },
                solicitacao
            );
        }

        // PUT: api/solicitacoes/1
        // Atualizar solicitação

        /// <summary>
        /// Atualiza uma solicitação existente.
        /// </summary>
        /// <param name="id">ID da solicitação.</param>
        /// <param name="updatedSolicitacao">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Solicitação atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Solicitação não encontrada.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSolicitacao(int id, Solicitacao updatedSolicitacao)
        {
            if (id != updatedSolicitacao.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingSolicitacao = await _context.Solicitacoes
                .FindAsync(id);

            if (existingSolicitacao == null)
            {
                return NotFound("Solicitação não encontrada.");
            }

            existingSolicitacao.Status = updatedSolicitacao.Status;
            existingSolicitacao.Mensagem = updatedSolicitacao.Mensagem;
            existingSolicitacao.RespondidoEm = updatedSolicitacao.RespondidoEm;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/solicitacoes/1
        // Remover solicitação

        /// <summary>
        /// Remove uma solicitação.
        /// </summary>
        /// <param name="id">ID da solicitação.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Solicitação removida com sucesso.</response>
        /// <response code="404">Solicitação não encontrada.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSolicitacao(int id)
        {
            var solicitacao = await _context.Solicitacoes
                .FindAsync(id);

            if (solicitacao == null)
            {
                return NotFound("Solicitação não encontrada.");
            }

            _context.Solicitacoes.Remove(solicitacao);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}