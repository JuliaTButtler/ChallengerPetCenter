using ChallengerPetCenter.Common;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class PetService : IPetService
    {
        private readonly AppDbContext _context;

        public PetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PetResponseDto>> GetAllAsync()
        {
            var pets = await _context.Pets.ToListAsync();
            return pets.Select(MapToDto);
        }

        public async Task<ServiceResult<PetResponseDto>> GetByIdAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return ServiceResult<PetResponseDto>.AsNotFound("Pet não encontrado.");

            return ServiceResult<PetResponseDto>.Success(MapToDto(pet));
        }

        public async Task<IEnumerable<PetResponseDto>> GetByUserAsync(int userId)
        {
            var pets = await _context.Pets
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return pets.Select(MapToDto);
        }

        public async Task<IEnumerable<PetResponseDto>> GetByEspecieAsync(string especie)
        {
            var pets = await _context.Pets
                .Where(p => p.Especie.ToUpper() == especie.ToUpper())
                .ToListAsync();

            return pets.Select(MapToDto);
        }

        public async Task<IEnumerable<PetResponseDto>> GetByNomeAsync(string nome)
        {
            var pets = await _context.Pets
                .Where(p => p.Nome.ToUpper().Contains(nome.ToUpper()))
                .ToListAsync();

            return pets.Select(MapToDto);
        }

        public async Task<ServiceResult<PetResponseDto>> CreateAsync(CreatePetDto dto)
        {
            var tutorExiste = await _context.Users
                .CountAsync(u => u.Id == dto.UserId) > 0;

            if (!tutorExiste)
                return ServiceResult<PetResponseDto>.Failure("Tutor não encontrado.");

            var pet = new Pet
            {
                Nome = dto.Nome,
                Especie = dto.Especie,
                Raca = dto.Raca,
                DataNascimento = dto.DataNascimento,
                Observacoes = dto.Observacoes,
                UserId = dto.UserId
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return ServiceResult<PetResponseDto>.Success(MapToDto(pet));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existingPet = await _context.Pets.FindAsync(id);
            if (existingPet == null)
                return ServiceResult.AsNotFound("Pet não encontrado.");

            var tutorExiste = await _context.Users
                .CountAsync(u => u.Id == dto.UserId) > 0;

            if (!tutorExiste)
                return ServiceResult.Failure("Tutor não encontrado.");

            existingPet.Nome = dto.Nome;
            existingPet.Especie = dto.Especie;
            existingPet.Raca = dto.Raca;
            existingPet.DataNascimento = dto.DataNascimento;
            existingPet.Observacoes = dto.Observacoes;
            existingPet.UserId = dto.UserId;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return ServiceResult.AsNotFound("Pet não encontrado.");

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return ServiceResult.Success();
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
