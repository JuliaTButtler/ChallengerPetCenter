using ChallengerPetCenter.Data;
using ChallengerPetCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: api/users
        // Lista todos os usuários
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .ToListAsync();

            return Ok(users);
        }

        // =====================================================
        // GET: api/users/1
        // Buscar usuário por ID
        // =====================================================

        [HttpGet("{id}")]
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

        // =====================================================
        // GET: api/users/email/teste@email.com
        // Buscar usuário por email
        // =====================================================

        [HttpGet("email/{email}")]
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

        // =====================================================
        // GET: api/users/tipo/TUTOR
        // Buscar usuários por tipo
        // =====================================================

        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByTipo(string tipo)
        {
            var users = await _context.Users
                .Where(u => u.TipoUsuario == tipo)
                .ToListAsync();

            if (!users.Any())
            {
                return NotFound("Nenhum usuário encontrado.");
            }

            return Ok(users);
        }

        // =====================================================
        // POST: api/users
        // Criar usuário
        // =====================================================

        [HttpPost]
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

        // =====================================================
        // PUT: api/users/1
        // Atualizar usuário
        // =====================================================

        [HttpPut("{id}")]
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

        // =====================================================
        // DELETE: api/users/1
        // Remover usuário
        // =====================================================

        [HttpDelete("{id}")]
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