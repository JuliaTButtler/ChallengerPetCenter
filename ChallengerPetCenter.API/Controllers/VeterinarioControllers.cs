using ChallengerPetCenter.DTOs.Veterinarios;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VeterinariosController : ApiControllerBase
    {
        private readonly IVeterinarioService _veterinarioService;

        public VeterinariosController(IVeterinarioService veterinarioService)
        {
            _veterinarioService = veterinarioService;
        }

        /// <summary>
        /// Lista todos os veterinários cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VeterinarioResponseDto>>> GetVeterinarios()
        {
            var veterinarios = await _veterinarioService.GetAllAsync();
            return Ok(veterinarios);
        }

        /// <summary>
        /// Busca um veterinário pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VeterinarioResponseDto>> GetVeterinarioById(int id)
        {
            var result = await _veterinarioService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca um veterinário pelo CRMV.
        /// </summary>
        [HttpGet("crmv/{crmv}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VeterinarioResponseDto>> GetVeterinarioByCrmv(string crmv)
        {
            var result = await _veterinarioService.GetByCrmvAsync(crmv);
            return ToActionResult(result);
        }

        /// <summary>
        /// Busca veterinários por especialidade.
        /// </summary>
        [HttpGet("especialidade/{especialidade}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VeterinarioResponseDto>>> GetVeterinariosByEspecialidade(string especialidade)
        {
            var veterinarios = (await _veterinarioService.GetByEspecialidadeAsync(especialidade)).ToList();
            if (!veterinarios.Any())
                return ProblemNotFound("Nenhum veterinário encontrado.");

            return Ok(veterinarios);
        }

        /// <summary>
        /// Busca um veterinário pelo ID do usuário.
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VeterinarioResponseDto>> GetVeterinarioByUserId(int userId)
        {
            var result = await _veterinarioService.GetByUserIdAsync(userId);
            return ToActionResult(result);
        }

        /// <summary>
        /// Cria um novo veterinário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VeterinarioResponseDto>> CreateVeterinario(CreateVeterinarioDto dto)
        {
            var result = await _veterinarioService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            return CreatedAtAction(
                nameof(GetVeterinarioById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        /// <summary>
        /// Atualiza um veterinário existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVeterinario(int id, UpdateVeterinarioDto dto)
        {
            var result = await _veterinarioService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um veterinário.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVeterinario(int id)
        {
            var result = await _veterinarioService.DeleteAsync(id);
            return ToActionResult(result);
        }
    }
}
