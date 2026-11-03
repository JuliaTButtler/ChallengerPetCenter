using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetVeterinariosController : ControllerBase
    {
        private readonly IPetVeterinarioService _petVeterinarioService;

        public PetVeterinariosController(IPetVeterinarioService petVeterinarioService)
        {
            _petVeterinarioService = petVeterinarioService;
        }

        /// <summary>
        /// Lista todos os vínculos entre pets e veterinários.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetPetVeterinarios()
        {
            var items = await _petVeterinarioService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Busca um vínculo pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetVeterinarioResponseDto>> GetPetVeterinarioById(int id)
        {
            var result = await _petVeterinarioService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca vínculos de um pet.
        /// </summary>
        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetByPet(int petId)
        {
            var items = (await _petVeterinarioService.GetByPetAsync(petId)).ToList();
            if (!items.Any())
                return NotFound("Nenhum vínculo encontrado.");

            return Ok(items);
        }

        /// <summary>
        /// Busca vínculos de um veterinário.
        /// </summary>
        [HttpGet("veterinario/{veterinarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetByVeterinario(int veterinarioId)
        {
            var items = (await _petVeterinarioService.GetByVeterinarioAsync(veterinarioId)).ToList();
            if (!items.Any())
                return NotFound("Nenhum vínculo encontrado.");

            return Ok(items);
        }

        /// <summary>
        /// Busca vínculos ativos.
        /// </summary>
        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetAtivos()
        {
            var items = (await _petVeterinarioService.GetAtivosAsync()).ToList();
            if (!items.Any())
                return NotFound("Nenhum vínculo ativo encontrado.");

            return Ok(items);
        }

        /// <summary>
        /// Cria um novo vínculo entre pet e veterinário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PetVeterinarioResponseDto>> CreatePetVeterinario(CreatePetVeterinarioDto dto)
        {
            var result = await _petVeterinarioService.CreateAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetPetVeterinarioById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza um vínculo existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePetVeterinario(int id, UpdatePetVeterinarioDto dto)
        {
            var result = await _petVeterinarioService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um vínculo.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePetVeterinario(int id)
        {
            var result = await _petVeterinarioService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private ActionResult<T> ToActionResult<T>(ServiceResult<T> result)
        {
            if (!result.Succeeded)
                return result.NotFound ? NotFound(result.Error) : BadRequest(result.Error);

            return Ok(result.Data);
        }

        private IActionResult ToActionResult(ServiceResult result)
        {
            if (!result.Succeeded)
                return result.NotFound ? NotFound(result.Error) : BadRequest(result.Error);

            return NoContent();
        }
    }
}
