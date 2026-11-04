using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.Veterinarios;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VeterinariosController : ApiControllerBase
    {
        private const string BasePath = "/api/veterinarios";
        private readonly IVeterinarioService _veterinarioService;

        public VeterinariosController(IVeterinarioService veterinarioService)
        {
            _veterinarioService = veterinarioService;
        }

        /// <summary>
        /// Lista veterinários com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<VeterinarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<VeterinarioResponseDto>>> GetVeterinarios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "asc",
            [FromQuery] string? especialidade = null,
            [FromQuery] int? userId = null)
        {
            var veterinarios = await _veterinarioService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(especialidade))
            {
                veterinarios = veterinarios.Where(v =>
                    v.Especialidade != null &&
                    string.Equals(v.Especialidade, especialidade, StringComparison.OrdinalIgnoreCase));
            }

            if (userId.HasValue)
            {
                veterinarios = veterinarios.Where(v => v.UserId == userId.Value);
            }

            veterinarios = SortVeterinarios(veterinarios, sort, order);
            var (items, p, size, total) = PagingHelper.Page(veterinarios, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["especialidade"] = especialidade,
                ["userId"] = userId?.ToString()
            };

            return Ok(new PagedResponseDto<VeterinarioResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca um veterinário pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<VeterinarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<VeterinarioResponseDto>>> GetVeterinarioById(int id)
        {
            var result = await _veterinarioService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Veterinário não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, id));
        }

        /// <summary>
        /// Busca um veterinário pelo CRMV.
        /// </summary>
        [HttpGet("crmv/{crmv}")]
        [ProducesResponseType(typeof(ResourceResponseDto<VeterinarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<VeterinarioResponseDto>>> GetVeterinarioByCrmv(string crmv)
        {
            var result = await _veterinarioService.GetByCrmvAsync(crmv);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Veterinário não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, result.Data!.Id));
        }

        /// <summary>
        /// Busca veterinários por especialidade.
        /// </summary>
        [HttpGet("especialidade/{especialidade}")]
        [ProducesResponseType(typeof(PagedResponseDto<VeterinarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<VeterinarioResponseDto>>> GetVeterinariosByEspecialidade(
            string especialidade,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetVeterinarios(page, pageSize, "id", "asc", especialidade, null);
        }

        /// <summary>
        /// Busca um veterinário pelo ID do usuário.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<VeterinarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<VeterinarioResponseDto>>> GetVeterinarioByUserId(int userId)
        {
            var result = await _veterinarioService.GetByUserIdAsync(userId);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Veterinário não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, result.Data!.Id));
        }

        /// <summary>
        /// Cria um novo veterinário.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<VeterinarioResponseDto>>> CreateVeterinario(CreateVeterinarioDto dto)
        {
            var result = await _veterinarioService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = ToResource(result.Data!, result.Data!.Id);
            return CreatedAtAction(nameof(GetVeterinarioById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza um veterinário existente.
        /// </summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVeterinario(int id, UpdateVeterinarioDto dto)
        {
            var result = await _veterinarioService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um veterinário.
        /// </summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVeterinario(int id)
        {
            var result = await _veterinarioService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<VeterinarioResponseDto> SortVeterinarios(
            IEnumerable<VeterinarioResponseDto> veterinarios,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "crmv" => PagingHelper.ApplySort(veterinarios, v => v.Crmv, order),
                "especialidade" => PagingHelper.ApplySort(veterinarios, v => v.Especialidade ?? string.Empty, order),
                "userid" => PagingHelper.ApplySort(veterinarios, v => v.UserId, order),
                _ => PagingHelper.ApplySort(veterinarios, v => v.Id, order)
            };
        }

        private static ResourceResponseDto<VeterinarioResponseDto> ToResource(
            VeterinarioResponseDto data,
            int id)
            => new()
            {
                Data = data,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            };
    }
}
