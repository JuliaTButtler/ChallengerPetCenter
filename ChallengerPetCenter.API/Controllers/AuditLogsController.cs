using ChallengerPetCenter.DTOs.AuditLogs;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    [Produces("application/json")]
    public class AuditLogsController : ApiControllerBase
    {
        private const string BasePath = "/api/audit-logs";
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Consulta a trilha de auditoria persistida no MongoDB (paginação + HATEOAS).
        /// </summary>
        /// <param name="entity">Filtro por entidade (ex.: User, Pet, DiarioEntrada).</param>
        /// <param name="entityId">Filtro por ID da entidade.</param>
        /// <param name="action">Filtro por ação (ex.: USER_CREATED, PET_UPDATED).</param>
        /// <param name="page">Página (1-based).</param>
        /// <param name="pageSize">Itens por página (máx. 100).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        [HttpGet]
        [ProducesResponseType(typeof(AuditLogListResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuditLogListResponseDto>> GetAuditLogs(
            [FromQuery] string? entity = null,
            [FromQuery] string? entityId = null,
            [FromQuery] string? action = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _auditLogService.QueryAsync(
                entity,
                entityId,
                action,
                page,
                pageSize,
                cancellationToken);

            var extra = new Dictionary<string, string?>
            {
                ["entity"] = entity,
                ["entityId"] = entityId,
                ["action"] = action
            };

            result.Links = HateoasLinkBuilder.ForPagedCollection(
                BasePath,
                result.Page,
                result.PageSize,
                result.Total,
                extra);

            return Ok(result);
        }
    }
}
