using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Pets;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _pets;
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;

        public PetService(
            IPetRepository pets,
            IUserRepository users,
            IUnitOfWork unitOfWork)
        {
            _pets = pets;
            _users = users;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PetResponseDto>> GetAllAsync()
        {
            var pets = await _pets.GetAllAsync();
            return pets.Select(MapToDto);
        }

        public async Task<ServiceResult<PetResponseDto>> GetByIdAsync(int id)
        {
            var pet = await _pets.GetByIdAsync(id);
            if (pet == null)
                return ServiceResult<PetResponseDto>.AsNotFound("Pet não encontrado.");

            return ServiceResult<PetResponseDto>.Success(MapToDto(pet));
        }

        public async Task<IEnumerable<PetResponseDto>> GetByUserAsync(int userId)
        {
            var pets = await _pets.GetByUserAsync(userId);
            return pets.Select(MapToDto);
        }

        public async Task<IEnumerable<PetResponseDto>> GetByEspecieAsync(string especie)
        {
            var pets = await _pets.GetByEspecieAsync(especie);
            return pets.Select(MapToDto);
        }

        public async Task<IEnumerable<PetResponseDto>> GetByNomeAsync(string nome)
        {
            var pets = await _pets.GetByNomeAsync(nome);
            return pets.Select(MapToDto);
        }

        public async Task<ServiceResult<PetResponseDto>> CreateAsync(CreatePetDto dto)
        {
            if (!await _users.ExistsAsync(dto.UserId))
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

            await _pets.AddAsync(pet);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<PetResponseDto>.Success(MapToDto(pet));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdatePetDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existingPet = await _pets.GetByIdAsync(id);
            if (existingPet == null)
                return ServiceResult.AsNotFound("Pet não encontrado.");

            if (!await _users.ExistsAsync(dto.UserId))
                return ServiceResult.Failure("Tutor não encontrado.");

            existingPet.Nome = dto.Nome;
            existingPet.Especie = dto.Especie;
            existingPet.Raca = dto.Raca;
            existingPet.DataNascimento = dto.DataNascimento;
            existingPet.Observacoes = dto.Observacoes;
            existingPet.UserId = dto.UserId;

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var pet = await _pets.GetByIdAsync(id);
            if (pet == null)
                return ServiceResult.AsNotFound("Pet não encontrado.");

            _pets.Remove(pet);
            await _unitOfWork.SaveChangesAsync();
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
