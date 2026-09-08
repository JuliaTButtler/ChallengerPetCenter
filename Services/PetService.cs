using ChallengerPetCenter.Data;
using ChallengerPetCenter.Domain.Rules;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class PetService : IPetService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PetService> _logger;

        public PetService(AppDbContext context, ILogger<PetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<PetResponseDto>>> GetAllAsync()
        {
            var pets = await _context.Pets.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<PetResponseDto>>.Ok(pets.Select(MapToDto));
        }

        public async Task<ServiceResult<PetResponseDto>> GetByIdAsync(int id)
        {
            var pet = await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (pet is null)
            {
                return ServiceResult<PetResponseDto>.NotFound("Pet não encontrado.");
            }

            return ServiceResult<PetResponseDto>.Ok(MapToDto(pet));
        }

        public async Task<ServiceResult<IEnumerable<PetResponseDto>>> GetByUserAsync(int userId)
        {
            var pets = await _context.Pets.AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (pets.Count == 0)
            {
                return ServiceResult<IEnumerable<PetResponseDto>>.NotFound("Nenhum pet encontrado para este tutor.");
            }

            return ServiceResult<IEnumerable<PetResponseDto>>.Ok(pets.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<PetResponseDto>>> GetByEspecieAsync(string especie)
        {
            var pets = await _context.Pets.AsNoTracking()
                .Where(p => p.Especie.ToUpper() == especie.ToUpper())
                .ToListAsync();

            if (pets.Count == 0)
            {
                return ServiceResult<IEnumerable<PetResponseDto>>.NotFound("Nenhum pet encontrado.");
            }

            return ServiceResult<IEnumerable<PetResponseDto>>.Ok(pets.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<PetResponseDto>>> GetByNomeAsync(string nome)
        {
            var pets = await _context.Pets.AsNoTracking()
                .Where(p => p.Nome.ToUpper().Contains(nome.ToUpper()))
                .ToListAsync();

            if (pets.Count == 0)
            {
                return ServiceResult<IEnumerable<PetResponseDto>>.NotFound("Nenhum pet encontrado.");
            }

            return ServiceResult<IEnumerable<PetResponseDto>>.Ok(pets.Select(MapToDto));
        }

        public async Task<ServiceResult<PetResponseDto>> CreateAsync(CreatePetDto dto)
        {
            var validationError = PetRules.ValidarCriacao(dto.Nome, dto.Especie);
            if (validationError is not null)
            {
                return ServiceResult<PetResponseDto>.BadRequest(validationError);
            }

            var tutorExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!tutorExists)
            {
                return ServiceResult<PetResponseDto>.BadRequest("Tutor não encontrado.");
            }

            var pet = new Pet
            {
                Nome = dto.Nome.Trim(),
                Especie = dto.Especie.Trim(),
                Raca = dto.Raca,
                DataNascimento = dto.DataNascimento,
                Observacoes = dto.Observacoes,
                UserId = dto.UserId
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pet {PetId} criado para tutor {UserId}", pet.Id, pet.UserId);
            return ServiceResult<PetResponseDto>.Ok(MapToDto(pet));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existingPet = await _context.Pets.FindAsync(id);
            if (existingPet is null)
            {
                return ServiceResult.NotFound("Pet não encontrado.");
            }

            existingPet.Nome = dto.Nome;
            existingPet.Especie = dto.Especie;
            existingPet.Raca = dto.Raca;
            existingPet.DataNascimento = dto.DataNascimento;
            existingPet.Observacoes = dto.Observacoes;
            existingPet.UserId = dto.UserId;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Pet {PetId} atualizado", id);
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet is null)
            {
                return ServiceResult.NotFound("Pet não encontrado.");
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            _logger.LogWarning("Pet {PetId} removido", id);
            return ServiceResult.Ok();
        }

        private static PetResponseDto MapToDto(Pet pet) => new()
        {
            Id = pet.Id,
            Nome = pet.Nome,
            Especie = pet.Especie,
            Raca = pet.Raca,
            DataNascimento = pet.DataNascimento,
            Observacoes = pet.Observacoes,
            UserId = pet.UserId
        };
    }
}
