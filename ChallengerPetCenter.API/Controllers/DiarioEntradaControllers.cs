using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiarioEntradasController : ApiControllerBase
    {
        private const string BasePath = "/api/diarioentradas";
        private readonly IDiarioEntradaService _diarioEntradaService;

        public DiarioEntradasController(IDiarioEntradaService diarioEntradaService)
        {
            _diarioEntradaService = diarioEntradaService;
        }

        /// <summary>
        /// Lista entradas do diário com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<DiarioEntradaResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<DiarioEntradaResponseDto>>> GetDiarioEntradas(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "data",
            [FromQuery] string? order = "desc",
            [FromQuery] string? status = null,
            [FromQuery] string? humor = null,
            [FromQuery] int? petId = null)
        {
            var entradas = await _diarioEntradaService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(status))
            {
                entradas = entradas.Where(e =>
                    e.Status != null &&
                    string.Equals(e.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(humor))
            {
                entradas = entradas.Where(e =>
                    e.HumorGeral != null &&
                    string.Equals(e.HumorGeral, humor, StringComparison.OrdinalIgnoreCase));
            }

            if (petId.HasValue)
            {
                entradas = entradas.Where(e => e.PetId == petId.Value);
            }

            entradas = SortEntradas(entradas, sort, order);
            var (items, p, size, total) = PagingHelper.Page(entradas, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["status"] = status,
                ["humor"] = humor,
                ["petId"] = petId?.ToString()
            };

            return Ok(new PagedResponseDto<DiarioEntradaResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca uma entrada pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<DiarioEntradaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<DiarioEntradaResponseDto>>> GetDiarioEntradaById(int id)
        {
            var result = await _diarioEntradaService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Entrada não encontrada.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(new ResourceResponseDto<DiarioEntradaResponseDto>
            {
                Data = result.Data!,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            });
        }

        /// <summary>
        /// Busca entradas de um pet.
        /// </summary>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<DiarioEntradaResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<DiarioEntradaResponseDto>>> GetEntradasByPet(
            int petId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetDiarioEntradas(page, pageSize, "data", "desc", null, null, petId);
        }

        /// <summary>
        /// Busca entradas por status.
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(PagedResponseDto<DiarioEntradaResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<DiarioEntradaResponseDto>>> GetEntradasByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetDiarioEntradas(page, pageSize, "data", "desc", status, null, null);
        }

        /// <summary>
        /// Busca entradas por humor geral.
        /// </summary>
        [HttpGet("humor/{humor}")]
        [ProducesResponseType(typeof(PagedResponseDto<DiarioEntradaResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<DiarioEntradaResponseDto>>> GetEntradasByHumor(
            string humor,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetDiarioEntradas(page, pageSize, "data", "desc", null, humor, null);
        }

        /// <summary>
        /// Cria uma nova entrada no diário.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<DiarioEntradaResponseDto>>> CreateDiarioEntrada(
            CreateDiarioEntradaDto dto)
        {
            var result = await _diarioEntradaService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = new ResourceResponseDto<DiarioEntradaResponseDto>
            {
                Data = result.Data!,
                Links = HateoasLinkBuilder.ForResource(BasePath, result.Data!.Id)
            };

            return CreatedAtAction(nameof(GetDiarioEntradaById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza uma entrada do diário.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDiarioEntrada(int id, UpdateDiarioEntradaDto dto)
        {
            var result = await _diarioEntradaService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove uma entrada do diário.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDiarioEntrada(int id)
        {
            var result = await _diarioEntradaService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<DiarioEntradaResponseDto> SortEntradas(
            IEnumerable<DiarioEntradaResponseDto> entradas,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "id" => PagingHelper.ApplySort(entradas, e => e.Id, order),
                "status" => PagingHelper.ApplySort(entradas, e => e.Status ?? string.Empty, order),
                "humorgeneral" or "humor" => PagingHelper.ApplySort(entradas, e => e.HumorGeral ?? string.Empty, order),
                "petid" => PagingHelper.ApplySort(entradas, e => e.PetId, order),
                _ => PagingHelper.ApplySort(entradas, e => e.Data, order)
            };
        }
    }
}
