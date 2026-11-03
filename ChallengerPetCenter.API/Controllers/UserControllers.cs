using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ApiControllerBase
    {
        private const string BasePath = "/api/users";
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lista usuários com paginação, ordenação e filtro.
        /// </summary>
        /// <param name="page">Página (1-based).</param>
        /// <param name="pageSize">Itens por página (máx. 100).</param>
        /// <param name="sort">Campo: id, nome, email, tipoUsuario.</param>
        /// <param name="order">asc ou desc.</param>
        /// <param name="tipoUsuario">Filtro por tipo (TUTOR, VETERINARIO).</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<UserResponseDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "asc",
            [FromQuery] string? tipoUsuario = null)
        {
            var users = await _userService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(tipoUsuario))
            {
                users = users.Where(u =>
                    string.Equals(u.TipoUsuario, tipoUsuario, StringComparison.OrdinalIgnoreCase));
            }

            users = SortUsers(users, sort, order);
            var (items, p, size, total) = PagingHelper.Page(users, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["tipoUsuario"] = tipoUsuario
            };

            return Ok(new PagedResponseDto<UserResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<UserResponseDto>>> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Usuário não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, BasePath, id));
        }

        /// <summary>
        /// Busca um usuário pelo email.
        /// </summary>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ResourceResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<UserResponseDto>>> GetUserByEmail(string email)
        {
            var result = await _userService.GetByEmailAsync(email);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Usuário não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(ToResource(result.Data!, BasePath, result.Data!.Id));
        }

        /// <summary>
        /// Busca usuários pelo tipo.
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(typeof(PagedResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<UserResponseDto>>> GetUsersByTipo(
            string tipo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetUsers(page, pageSize, "id", "asc", tipo);
        }

        /// <summary>
        /// Cria um novo usuário (público).
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResourceResponseDto<UserResponseDto>>> CreateUser(CreateUserDto dto)
        {
            var result = await _userService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = ToResource(result.Data!, BasePath, result.Data!.Id);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um usuário. Requer autenticação JWT.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<UserResponseDto> SortUsers(
            IEnumerable<UserResponseDto> users,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "nome" => PagingHelper.ApplySort(users, u => u.Nome, order),
                "email" => PagingHelper.ApplySort(users, u => u.Email, order),
                "tipousuario" => PagingHelper.ApplySort(users, u => u.TipoUsuario, order),
                _ => PagingHelper.ApplySort(users, u => u.Id, order)
            };
        }

        private static ResourceResponseDto<UserResponseDto> ToResource(
            UserResponseDto data,
            string basePath,
            int id)
            => new()
            {
                Data = data,
                Links = HateoasLinkBuilder.ForResource(basePath, id)
            };
    }
}
