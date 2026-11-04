using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Comentarios;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ComentariosController : ApiControllerBase
    {
        private const string BasePath = "/api/comentarios";
        private readonly IComentarioService _comentarioService;

        public ComentariosController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        /// <summary>
        /// Lista comentários com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<ComentarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<ComentarioResponseDto>>> GetComentarios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "desc",
            [FromQuery] int? diarioEntradaId = null,
            [FromQuery] int? userId = null,
            [FromQuery] string? texto = null)
        {
            var comentarios = await _comentarioService.GetAllAsync();

            if (diarioEntradaId.HasValue)
            {
                comentarios = comentarios.Where(c => c.DiarioEntradaId == diarioEntradaId.Value);
            }

            if (userId.HasValue)
            {
                comentarios = comentarios.Where(c => c.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(texto))
            {
                comentarios = comentarios.Where(c =>
                    c.TextoComentario.Contains(texto, StringComparison.OrdinalIgnoreCase));
            }

            comentarios = SortComentarios(comentarios, sort, order);
            var (items, p, size, total) = PagingHelper.Page(comentarios, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["diarioEntradaId"] = diarioEntradaId?.ToString(),
                ["userId"] = userId?.ToString(),
                ["texto"] = texto
            };

            return Ok(new PagedResponseDto<ComentarioResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca um comentário pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<ComentarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<ComentarioResponseDto>>> GetComentarioById(int id)
        {
            var result = await _comentarioService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Comentário não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, id));
        }

        /// <summary>
        /// Busca comentários de uma entrada do diário.
        /// </summary>
        [HttpGet("diario/{diarioEntradaId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<ComentarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<ComentarioResponseDto>>> GetComentariosByDiario(
            int diarioEntradaId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetComentarios(page, pageSize, "id", "desc", diarioEntradaId, null, null);
        }

        /// <summary>
        /// Busca comentários de um usuário.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<ComentarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<ComentarioResponseDto>>> GetComentariosByUser(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetComentarios(page, pageSize, "id", "desc", null, userId, null);
        }

        /// <summary>
        /// Busca comentários pelo texto.
        /// </summary>
        [HttpGet("texto/{texto}")]
        [ProducesResponseType(typeof(PagedResponseDto<ComentarioResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<ComentarioResponseDto>>> GetComentariosByTexto(
            string texto,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetComentarios(page, pageSize, "id", "desc", null, null, texto);
        }

        /// <summary>
        /// Cria um novo comentário.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<ComentarioResponseDto>>> CreateComentario(CreateComentarioDto dto)
        {
            var result = await _comentarioService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = ToResource(result.Data!, result.Data!.Id);
            return CreatedAtAction(nameof(GetComentarioById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza um comentário existente.
        /// </summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComentario(int id, UpdateComentarioDto dto)
        {
            var result = await _comentarioService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um comentário.
        /// </summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var result = await _comentarioService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<ComentarioResponseDto> SortComentarios(
            IEnumerable<ComentarioResponseDto> comentarios,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "criadoem" => PagingHelper.ApplySort(comentarios, c => c.CriadoEm, order),
                "userid" => PagingHelper.ApplySort(comentarios, c => c.UserId, order),
                "diarioentradaid" => PagingHelper.ApplySort(comentarios, c => c.DiarioEntradaId, order),
                _ => PagingHelper.ApplySort(comentarios, c => c.Id, order)
            };
        }

        private static ResourceResponseDto<ComentarioResponseDto> ToResource(
            ComentarioResponseDto data,
            int id)
            => new()
            {
                Data = data,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            };
    }
}
