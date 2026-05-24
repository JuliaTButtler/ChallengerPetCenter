using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/pets
        // Lista todos os pets

        /// <summary>
        /// Lista todos os pets cadastrados.
        /// </summary>
        /// <returns>Lista de pets.</returns>
        /// <response code="200">Pets encontrados com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets()
        {
            var pets = await _context.Pets
                .ToListAsync();

            return Ok(pets);
        }

        // GET: api/pets/1
        // Buscar pet por ID

        /// <summary>
        /// Busca um pet pelo ID.
        /// </summary>
        /// <param name="id">ID do pet.</param>
        /// <returns>Pet encontrado.</returns>
        /// <response code="200">Pet encontrado.</response>
        /// <response code="404">Pet não encontrado.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pet>> GetPetById(int id)
        {
            var pet = await _context.Pets
                .FindAsync(id);

            if (pet == null)
            {
                return NotFound("Pet não encontrado.");
            }

            return Ok(pet);
        }

        // GET: api/pets/user/1
        // Buscar pets de um tutor

        /// <summary>
        /// Busca todos os pets de um tutor.
        /// </summary>
        /// <param name="userId">ID do tutor.</param>
        /// <returns>Lista de pets do tutor.</returns>
        /// <response code="200">Pets encontrados.</response>
        /// <response code="404">Nenhum pet encontrado.</response>

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByUser(int userId)
        {
            var pets = await _context.Pets
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (!pets.Any())
            {
                return NotFound("Nenhum pet encontrado para este tutor.");
            }

            return Ok(pets);
        }

        // GET: api/pets/especie/cachorro
        // Buscar pets por espécie

        /// <summary>
        /// Busca pets por espécie.
        /// </summary>
        /// <param name="especie">Espécie do pet.</param>
        /// <returns>Lista de pets.</returns>
        /// <response code="200">Pets encontrados.</response>
        /// <response code="404">Nenhum pet encontrado.</response>

        [HttpGet("especie/{especie}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByEspecie(string especie)
        {
            var pets = await _context.Pets
                .Where(p => p.Especie.ToUpper() == especie.ToUpper())
                .ToListAsync();

            if (!pets.Any())
            {
                return NotFound("Nenhum pet encontrado.");
            }

            return Ok(pets);
        }

        // GET: api/pets/nome/Bob
        // Buscar pets por nome

        /// <summary>
        /// Busca pets pelo nome.
        /// </summary>
        /// <param name="nome">Nome do pet.</param>
        /// <returns>Lista de pets encontrados.</returns>
        /// <response code="200">Pets encontrados.</response>
        /// <response code="404">Nenhum pet encontrado.</response>

        [HttpGet("nome/{nome}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByNome(string nome)
        {
            var pets = await _context.Pets
                .Where(p => p.Nome.ToUpper().Contains(nome.ToUpper()))
                .ToListAsync();

            if (!pets.Any())
            {
                return NotFound("Nenhum pet encontrado.");
            }

            return Ok(pets);
        }

        // POST: api/pets
        // Criar pet

        /// <summary>
        /// Cria um novo pet.
        /// </summary>
        /// <param name="pet">Dados do pet.</param>
        /// <returns>Pet criado.</returns>
        /// <response code="201">Pet criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pet>> CreatePet(Pet pet)
        {
            if (string.IsNullOrWhiteSpace(pet.Nome))
            {
                return BadRequest("Nome é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(pet.Especie))
            {
                return BadRequest("Espécie é obrigatória.");
            }

            var userCount = await _context.Users
                .CountAsync(u => u.Id == pet.UserId);

            if (userCount == 0)
            {
                return BadRequest("Tutor não encontrado.");
            }

            _context.Pets.Add(pet);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPetById),
                new { id = pet.Id },
                pet
            );
        }

        // PUT: api/pets/1
        // Atualizar pet

        /// <summary>
        /// Atualiza os dados de um pet.
        /// </summary>
        /// <param name="id">ID do pet.</param>
        /// <param name="updatedPet">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Pet atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Pet não encontrado.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePet(int id, Pet updatedPet)
        {
            if (id != updatedPet.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingPet = await _context.Pets
                .FindAsync(id);

            if (existingPet == null)
            {
                return NotFound("Pet não encontrado.");
            }

            existingPet.Nome = updatedPet.Nome;
            existingPet.Especie = updatedPet.Especie;
            existingPet.Raca = updatedPet.Raca;
            existingPet.DataNascimento = updatedPet.DataNascimento;
            existingPet.Observacoes = updatedPet.Observacoes;
            existingPet.UserId = updatedPet.UserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/pets/1
        // Remover pet

        /// <summary>
        /// Remove um pet.
        /// </summary>
        /// <param name="id">ID do pet.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Pet removido com sucesso.</response>
        /// <response code="404">Pet não encontrado.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePet(int id)
        {
            var pet = await _context.Pets
                .FindAsync(id);

            if (pet == null)
            {
                return NotFound("Pet não encontrado.");
            }

            _context.Pets.Remove(pet);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}