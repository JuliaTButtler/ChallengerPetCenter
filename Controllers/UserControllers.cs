using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        // Lista todos os usuários

        /// <summary>
        /// Lista todos os usuários cadastrados.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        /// <response code="200">Usuários encontrados com sucesso.</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/1
        // Buscar usuário por ID

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Usuário encontrado.</returns>
        /// <response code="200">Usuário encontrado.</response>
        /// <response code="404">Usuário não encontrado.</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            return Ok(user);
        }

        // GET: api/users/email/teste@email.com
        // Buscar usuário por email

        /// <summary>
        /// Busca um usuário pelo email.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <returns>Usuário encontrado.</returns>
        /// <response code="200">Usuário encontrado.</response>
        /// <response code="404">Usuário não encontrado.</response>

        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            return Ok(user);
        }

        // GET: api/users/tipo/TUTOR
        // Buscar usuários por tipo

        /// <summary>
        /// Busca usuários pelo tipo.
        /// </summary>
        /// <param name="tipo">Tipo do usuário (TUTOR ou VETERINARIO).</param>
        /// <returns>Lista de usuários.</returns>
        /// <response code="200">Usuários encontrados.</response>
        /// <response code="404">Nenhum usuário encontrado.</response>

        [HttpGet("tipo/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByTipo(string tipo)
        {
            tipo = tipo.ToUpper();

            var users = await _context.Users
                .Where(u => u.TipoUsuario.ToUpper() == tipo)
                .ToListAsync();

            if (!users.Any())
            {
                return NotFound("Nenhum usuário encontrado.");
            }

            return Ok(users);
        }

        // POST: api/users
        // Criar usuário

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="user">Dados do usuário.</param>
        /// <returns>Usuário criado.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            // Validação simples

            if (string.IsNullOrWhiteSpace(user.Nome))
            {
                return BadRequest("Nome é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Email é obrigatório.");
            }

            // Verificar email duplicado

            var count = await _context.Users
                .CountAsync(u => u.Email == user.Email);

            if (count > 0)
            {
                return BadRequest("Email já cadastrado.");
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                user
            );
        }

        // PUT: api/users/1
        // Atualizar usuário

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <param name="updatedUser">Dados atualizados.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Usuário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Usuário não encontrado.</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingUser = await _context.Users
                .FindAsync(id);

            if (existingUser == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Atualização dos campos

            existingUser.Nome = updatedUser.Nome;
            existingUser.Email = updatedUser.Email;
            existingUser.Telefone = updatedUser.Telefone;
            existingUser.TipoUsuario = updatedUser.TipoUsuario;
            existingUser.Ativo = updatedUser.Ativo;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/1
        // Remover usuário

        /// <summary>
        /// Remove um usuário.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Usuário removido com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}