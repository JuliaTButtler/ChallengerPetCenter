using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.PetVeterinarios;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetVeterinariosController : ApiControllerBase
    {
        private const string BasePath = "/api/petveterinarios";
        private readonly IPetVeterinarioService _petVeterinarioService;

        public PetVeterinariosController(IPetVeterinarioService petVeterinarioService)
        {
            _petVeterinarioService = petVeterinarioService;
        }

        /// <summary>
        /// Lista vínculos pet–veterinário com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<PetVeterinarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetVeterinarioResponseDto>>> GetPetVeterinarios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "asc",
            [FromQuery] int? petId = null,
            [FromQuery] int? veterinarioId = null,
            [FromQuery] int? ativo = null)
        {
            var items = await _petVeterinarioService.GetAllAsync();

            if (petId.HasValue)
            {
                items = items.Where(x => x.PetId == petId.Value);
            }

            if (veterinarioId.HasValue)
            {
                items = items.Where(x => x.VeterinarioId == veterinarioId.Value);
            }

            if (ativo.HasValue)
            {
                items = items.Where(x => x.Ativo == ativo.Value);
            }

            items = SortPetVeterinarios(items, sort, order);
            var (pageItems, p, size, total) = PagingHelper.Page(items, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["petId"] = petId?.ToString(),
                ["veterinarioId"] = veterinarioId?.ToString(),
                ["ativo"] = ativo?.ToString()
            };

            return Ok(new PagedResponseDto<PetVeterinarioResponseDto>
            {
                Items = pageItems,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca um vínculo pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<PetVeterinarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<PetVeterinarioResponseDto>>> GetPetVeterinarioById(int id)
        {
            var result = await _petVeterinarioService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Vínculo não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, id));
        }

        /// <summary>
        /// Busca vínculos de um pet.
        /// </summary>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<PetVeterinarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetVeterinarioResponseDto>>> GetByPet(
            int petId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetPetVeterinarios(page, pageSize, "id", "asc", petId, null, null);
        }

        /// <summary>
        /// Busca vínculos de um veterinário.
        /// </summary>
        [HttpGet("veterinario/{veterinarioId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<PetVeterinarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetVeterinarioResponseDto>>> GetByVeterinario(
            int veterinarioId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetPetVeterinarios(page, pageSize, "id", "asc", null, veterinarioId, null);
        }

        /// <summary>
        /// Busca vínculos ativos.
        /// </summary>
        [HttpGet("ativos")]
        [ProducesResponseType(typeof(PagedResponseDto<PetVeterinarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetVeterinarioResponseDto>>> GetAtivos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetPetVeterinarios(page, pageSize, "id", "asc", null, null, 1);
        }

        /// <summary>
        /// Cria um novo vínculo entre pet e veterinário.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<PetVeterinarioResponseDto>>> CreatePetVeterinario(CreatePetVeterinarioDto dto)
        {
            var result = await _petVeterinarioService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = ToResource(result.Data!, result.Data!.Id);
            return CreatedAtAction(nameof(GetPetVeterinarioById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza um vínculo existente.
        /// </summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePetVeterinario(int id, UpdatePetVeterinarioDto dto)
        {
            var result = await _petVeterinarioService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um vínculo.
        /// </summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePetVeterinario(int id)
        {
            var result = await _petVeterinarioService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<PetVeterinarioResponseDto> SortPetVeterinarios(
            IEnumerable<PetVeterinarioResponseDto> items,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "datainicio" => PagingHelper.ApplySort(items, x => x.DataInicio, order),
                "ativo" => PagingHelper.ApplySort(items, x => x.Ativo, order),
                "petid" => PagingHelper.ApplySort(items, x => x.PetId, order),
                "veterinarioid" => PagingHelper.ApplySort(items, x => x.VeterinarioId, order),
                _ => PagingHelper.ApplySort(items, x => x.Id, order)
            };
        }

        private static ResourceResponseDto<PetVeterinarioResponseDto> ToResource(
            PetVeterinarioResponseDto data,
            int id)
            => new()
            {
                Data = data,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            };
    }
}
