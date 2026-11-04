using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RegistrosController : ApiControllerBase
    {
        private const string BasePath = "/api/registros";
        private readonly IRegistroService _registroService;

        public RegistrosController(IRegistroService registroService)
        {
            _registroService = registroService;
        }

        /// <summary>
        /// Lista registros com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<RegistroResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<RegistroResponseDto>>> GetRegistros(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "desc",
            [FromQuery] string? tipo = null,
            [FromQuery] string? subtipo = null,
            [FromQuery] int? diarioEntradaId = null)
        {
            var registros = await _registroService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                registros = registros.Where(r =>
                    string.Equals(r.Tipo, tipo, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(subtipo))
            {
                registros = registros.Where(r =>
                    r.Subtipo != null &&
                    string.Equals(r.Subtipo, subtipo, StringComparison.OrdinalIgnoreCase));
            }

            if (diarioEntradaId.HasValue)
            {
                registros = registros.Where(r => r.DiarioEntradaId == diarioEntradaId.Value);
            }

            registros = SortRegistros(registros, sort, order);
            var (items, p, size, total) = PagingHelper.Page(registros, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["tipo"] = tipo,
                ["subtipo"] = subtipo,
                ["diarioEntradaId"] = diarioEntradaId?.ToString()
            };

            return Ok(new PagedResponseDto<RegistroResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca um registro pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<RegistroResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<RegistroResponseDto>>> GetRegistroById(int id)
        {
            var result = await _registroService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Registro não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, id));
        }

        /// <summary>
        /// Busca registros de uma entrada do diário.
        /// </summary>
        [HttpGet("diario/{diarioEntradaId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<RegistroResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<RegistroResponseDto>>> GetRegistrosByDiario(
            int diarioEntradaId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetRegistros(page, pageSize, "id", "desc", null, null, diarioEntradaId);
        }

        /// <summary>
        /// Busca registros por tipo.
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(PagedResponseDto<RegistroResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<RegistroResponseDto>>> GetRegistrosByTipo(
            string tipo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetRegistros(page, pageSize, "id", "desc", tipo, null, null);
        }

        /// <summary>
        /// Busca registros por subtipo.
        /// </summary>
        [HttpGet("subtipo/{subtipo}")]
        [ProducesResponseType(typeof(PagedResponseDto<RegistroResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<RegistroResponseDto>>> GetRegistrosBySubtipo(
            string subtipo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetRegistros(page, pageSize, "id", "desc", null, subtipo, null);
        }

        /// <summary>
        /// Cria um novo registro.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<RegistroResponseDto>>> CreateRegistro(CreateRegistroDto dto)
        {
            var result = await _registroService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = ToResource(result.Data!, result.Data!.Id);
            return CreatedAtAction(nameof(GetRegistroById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza um registro existente.
        /// </summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRegistro(int id, UpdateRegistroDto dto)
        {
            var result = await _registroService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um registro.
        /// </summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRegistro(int id)
        {
            var result = await _registroService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<RegistroResponseDto> SortRegistros(
            IEnumerable<RegistroResponseDto> registros,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "tipo" => PagingHelper.ApplySort(registros, r => r.Tipo, order),
                "subtipo" => PagingHelper.ApplySort(registros, r => r.Subtipo ?? string.Empty, order),
                "horario" => PagingHelper.ApplySort(registros, r => r.Horario ?? DateTime.MinValue, order),
                "diarioentradaid" => PagingHelper.ApplySort(registros, r => r.DiarioEntradaId, order),
                _ => PagingHelper.ApplySort(registros, r => r.Id, order)
            };
        }

        private static ResourceResponseDto<RegistroResponseDto> ToResource(
            RegistroResponseDto data,
            int id)
            => new()
            {
                Data = data,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            };
    }
}
