using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Hateoas;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengerPetCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetsController : ApiControllerBase
    {
        private const string BasePath = "/api/pets";
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        /// <summary>
        /// Lista pets com paginação, ordenação e filtros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<PetResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetResponseDto>>> GetPets(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = "id",
            [FromQuery] string? order = "asc",
            [FromQuery] string? especie = null,
            [FromQuery] string? nome = null,
            [FromQuery] int? userId = null)
        {
            var pets = await _petService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(especie))
            {
                pets = pets.Where(p =>
                    string.Equals(p.Especie, especie, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                pets = pets.Where(p =>
                    p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
            }

            if (userId.HasValue)
            {
                pets = pets.Where(p => p.UserId == userId.Value);
            }

            pets = SortPets(pets, sort, order);
            var (items, p, size, total) = PagingHelper.Page(pets, page, pageSize);

            var extra = new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["order"] = order,
                ["especie"] = especie,
                ["nome"] = nome,
                ["userId"] = userId?.ToString()
            };

            return Ok(new PagedResponseDto<PetResponseDto>
            {
                Items = items,
                Page = p,
                PageSize = size,
                Total = total,
                Links = HateoasLinkBuilder.ForPagedCollection(BasePath, p, size, total, extra)
            });
        }

        /// <summary>
        /// Busca um pet pelo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResourceResponseDto<PetResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponseDto<PetResponseDto>>> GetPetById(int id)
        {
            var result = await _petService.GetByIdAsync(id);
            if (!result.Succeeded)
            {
                return result.NotFound
                    ? ProblemNotFound(result.Error ?? "Pet não encontrado.")
                    : ProblemBadRequest(result.Error ?? "Requisição inválida.");
            }

            return Ok(new ResourceResponseDto<PetResponseDto>
            {
                Data = result.Data!,
                Links = HateoasLinkBuilder.ForResource(BasePath, id)
            });
        }

        /// <summary>
        /// Busca todos os pets de um tutor.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(PagedResponseDto<PetResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetResponseDto>>> GetPetsByUser(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetPets(page, pageSize, "id", "asc", null, null, userId);
        }

        /// <summary>
        /// Busca pets por espécie.
        /// </summary>
        [HttpGet("especie/{especie}")]
        [ProducesResponseType(typeof(PagedResponseDto<PetResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetResponseDto>>> GetPetsByEspecie(
            string especie,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetPets(page, pageSize, "id", "asc", especie, null, null);
        }

        /// <summary>
        /// Busca pets pelo nome.
        /// </summary>
        [HttpGet("nome/{nome}")]
        [ProducesResponseType(typeof(PagedResponseDto<PetResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponseDto<PetResponseDto>>> GetPetsByNome(
            string nome,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetPets(page, pageSize, "id", "asc", null, nome, null);
        }

        /// <summary>
        /// Cria um novo pet.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResourceResponseDto<PetResponseDto>>> CreatePet(CreatePetDto dto)
        {
            var result = await _petService.CreateAsync(dto);
            if (!result.Succeeded)
                return ProblemBadRequest(result.Error ?? "Requisição inválida.");

            var resource = new ResourceResponseDto<PetResponseDto>
            {
                Data = result.Data!,
                Links = HateoasLinkBuilder.ForResource(BasePath, result.Data!.Id)
            };

            return CreatedAtAction(nameof(GetPetById), new { id = result.Data.Id }, resource);
        }

        /// <summary>
        /// Atualiza os dados de um pet.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePet(int id, UpdatePetDto dto)
        {
            var result = await _petService.UpdateAsync(id, dto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Remove um pet.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePet(int id)
        {
            var result = await _petService.DeleteAsync(id);
            return ToActionResult(result);
        }

        private static IEnumerable<PetResponseDto> SortPets(
            IEnumerable<PetResponseDto> pets,
            string? sort,
            string? order)
        {
            return (sort?.ToLowerInvariant()) switch
            {
                "nome" => PagingHelper.ApplySort(pets, p => p.Nome, order),
                "especie" => PagingHelper.ApplySort(pets, p => p.Especie, order),
                "userid" => PagingHelper.ApplySort(pets, p => p.UserId, order),
                _ => PagingHelper.ApplySort(pets, p => p.Id, order)
            };
        }
    }
}
