using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetsController : ApiControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        /// <summary>
        /// Lista todos os pets cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PetResponseDto>>> GetPets()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        /// <summary>
        /// Busca um pet pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetResponseDto>> GetPetById(int id)
        {
            var result = await _petService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca todos os pets de um tutor.
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetResponseDto>>> GetPetsByUser(int userId)
        {
            var pets = (await _petService.GetByUserAsync(userId)).ToList();
            if (!pets.Any())
                return ProblemNotFound("Nenhum pet encontrado para este tutor.");

            return Ok(pets);
        }

        /// <summary>
        /// Busca pets por espécie.
        /// </summary>
        [HttpGet("especie/{especie}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetResponseDto>>> GetPetsByEspecie(string especie)
        {
            var pets = (await _petService.GetByEspecieAsync(especie)).ToList();
            if (!pets.Any())
                return ProblemNotFound("Nenhum pet encontrado.");

            return Ok(pets);
        }

        /// <summary>
        /// Busca pets pelo nome.
        /// </summary>
        [HttpGet("nome/{nome}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetResponseDto>>> GetPetsByNome(string nome)
        {
            var pets = (await _petService.GetByNomeAsync(nome)).ToList();
            if (!pets.Any())
                return ProblemNotFound("Nenhum pet encontrado.");

            return Ok(pets);
        }

        /// <summary>
        /// Cria um novo pet.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PetResponseDto>> CreatePet(CreatePetDto dto)
        {
            var result = await _petService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            return CreatedAtAction(
                nameof(GetPetById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza os dados de um pet.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePet(int id, UpdatePetDto dto)
        {
            var result = await _petService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um pet.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePet(int id)
        {
            var result = await _petService.DeleteAsync(id);
            return ToActionResult(result);
        }
    }
}
