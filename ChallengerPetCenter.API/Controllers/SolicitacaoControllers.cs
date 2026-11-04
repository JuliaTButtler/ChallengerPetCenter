using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SolicitacoesController : ApiControllerBase
    {
        private const string BasePath = "/api/solicitacoes";
        private readonly ISolicitacaoService _solicitacaoService;

        public SolicitacoesController(ISolicitacaoService solicitacaoService)
        {
            _solicitacaoService = solicitacaoService;
        }

        /// <summary>
        /// Lista solicitações com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<SolicitacaoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<SolicitacaoResponseDto>>> GetSolicitacoes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "desc",
            [FromQuery] string? status = null,
            [FromQuery] int? petId = null,
            [FromQuery] int? tutorId = null,
            [FromQuery] int? veterinarioId = null)
        {
            var solicitacoes = await _solicitacaoService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(status))
            {
                solicitacoes = solicitacoes.Where(s =>
                    string.Equals(s.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (petId.HasValue)
            {
                solicitacoes = solicitacoes.Where(s => s.PetId == petId.Value);
            }

            if (tutorId.HasValue)
            {
                solicitacoes = solicitacoes.Where(s => s.TutorId == tutorId.Value);
            }

            if (veterinarioId.HasValue)
            {
                solicitacoes = solicitacoes.Where(s => s.VeterinarioId == veterinarioId.Value);
            }

            solicitacoes = SortSolicitacoes(solicitacoes, sort, order);
            var (items, p, size, total) = PagingHelper.Page(solicitacoes, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["status"] = status,
                ["petId"] = petId?.ToString(),
                ["tutorId"] = tutorId?.ToString(),
                ["veterinarioId"] = veterinarioId?.ToString()
            };

            return Ok(new PagedResponseDto<SolicitacaoResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca uma solicitação pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<SolicitacaoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<SolicitacaoResponseDto>>> GetSolicitacaoById(int id)
        {
            var result = await _solicitacaoService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Solicitação não encontrada.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, id));
        }

        /// <summary>
        /// Busca solicitações de um pet.
        /// </summary>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<SolicitacaoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<SolicitacaoResponseDto>>> GetSolicitacoesByPet(
            int petId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetSolicitacoes(page, pageSize, "id", "desc", null, petId, null, null);
        }

        /// <summary>
        /// Busca solicitações de um tutor.
        /// </summary>
        [HttpGet("tutor/{tutorId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<SolicitacaoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<SolicitacaoResponseDto>>> GetSolicitacoesByTutor(
            int tutorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetSolicitacoes(page, pageSize, "id", "desc", null, null, tutorId, null);
        }

        /// <summary>
        /// Busca solicitações de um veterinário.
        /// </summary>
        [HttpGet("veterinario/{veterinarioId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<SolicitacaoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<SolicitacaoResponseDto>>> GetSolicitacoesByVeterinario(
            int veterinarioId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetSolicitacoes(page, pageSize, "id", "desc", null, null, null, veterinarioId);
        }

        /// <summary>
        /// Busca solicitações por status.
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(PagedResponseDto<SolicitacaoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<SolicitacaoResponseDto>>> GetSolicitacoesByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetSolicitacoes(page, pageSize, "id", "desc", status, null, null, null);
        }

        /// <summary>
        /// Cria uma nova solicitação.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<SolicitacaoResponseDto>>> CreateSolicitacao(CreateSolicitacaoDto dto)
        {
            var result = await _solicitacaoService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = ToResource(result.Data!, result.Data!.Id);
            return CreatedAtAction(nameof(GetSolicitacaoById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza uma solicitação existente.
        /// </summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSolicitacao(int id, UpdateSolicitacaoDto dto)
        {
            var result = await _solicitacaoService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove uma solicitação.
        /// </summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSolicitacao(int id)
        {
            var result = await _solicitacaoService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<SolicitacaoResponseDto> SortSolicitacoes(
            IEnumerable<SolicitacaoResponseDto> solicitacoes,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "status" => PagingHelper.ApplySort(solicitacoes, s => s.Status, order),
                "criadoem" => PagingHelper.ApplySort(solicitacoes, s => s.CriadoEm, order),
                "petid" => PagingHelper.ApplySort(solicitacoes, s => s.PetId, order),
                "tutorid" => PagingHelper.ApplySort(solicitacoes, s => s.TutorId, order),
                "veterinarioid" => PagingHelper.ApplySort(solicitacoes, s => s.VeterinarioId, order),
                _ => PagingHelper.ApplySort(solicitacoes, s => s.Id, order)
            };
        }

        private static ResourceResponseDto<SolicitacaoResponseDto> ToResource(
            SolicitacaoResponseDto data,
            int id)
            => new()
            {
                Data = data,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            };
    }
}
