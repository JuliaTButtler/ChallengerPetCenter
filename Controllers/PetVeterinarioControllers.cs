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
            var result = await _petVeterinarioService.GetAllAsync();
            return this.ToActionResult(result);
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
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca vínculos de um pet.
        /// </summary>
        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetByPet(int petId)
        {
            var result = await _petVeterinarioService.GetByPetAsync(petId);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca vínculos de um veterinário.
        /// </summary>
        [HttpGet("veterinario/{veterinarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetByVeterinario(int veterinarioId)
        {
            var result = await _petVeterinarioService.GetByVeterinarioAsync(veterinarioId);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca vínculos ativos.
        /// </summary>
        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PetVeterinarioResponseDto>>> GetAtivos()
        {
            var result = await _petVeterinarioService.GetAtivosAsync();
            return this.ToActionResult(result);
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
            return this.ToCreatedResult(result, nameof(GetPetVeterinarioById), new { id = result.Data?.Id });
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
            return this.ToActionResult(result);
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
            return this.ToActionResult(result);
        }
    }
}
