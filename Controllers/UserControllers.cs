using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lista todos os usuários cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var result = await _userService.GetAllAsync();
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca um usuário pelo email.
        /// </summary>
        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
        {
            var result = await _userService.GetByEmailAsync(email);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Busca usuários pelo tipo.
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByTipo(string tipo)
        {
            var result = await _userService.GetByTipoAsync(tipo);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto)
        {
            var result = await _userService.CreateAsync(dto);
            return this.ToCreatedResult(result, nameof(GetUserById), new { id = result.Data?.Id });
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Remove um usuário.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);
            return this.ToActionResult(result);
        }
    }
}
