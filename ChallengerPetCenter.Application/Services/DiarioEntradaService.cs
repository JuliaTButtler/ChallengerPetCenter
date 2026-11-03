using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class DiarioEntradaService : IDiarioEntradaService
    {
        private readonly IDiarioEntradaRepository _diarioEntradas;
        private readonly IPetRepository _pets;
        private readonly IUnitOfWork _unitOfWork;

        public DiarioEntradaService(
            IDiarioEntradaRepository diarioEntradas,
            IPetRepository pets,
            IUnitOfWork unitOfWork)
        {
            _diarioEntradas = diarioEntradas;
            _pets = pets;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetAllAsync()
        {
            var entradas = await _diarioEntradas.GetAllAsync();
            return entradas.Select(MapToDto);
        }

        public async Task<ServiceResult<DiarioEntradaResponseDto>> GetByIdAsync(int id)
        {
            var entrada = await _diarioEntradas.GetByIdAsync(id);
            if (entrada == null)
                return ServiceResult<DiarioEntradaResponseDto>.AsNotFound("Entrada não encontrada.");

            return ServiceResult<DiarioEntradaResponseDto>.Success(MapToDto(entrada));
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetByPetAsync(int petId)
        {
            var entradas = await _diarioEntradas.GetByPetAsync(petId);
            return entradas.Select(MapToDto);
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetByStatusAsync(string status)
        {
            var entradas = await _diarioEntradas.GetByStatusAsync(status);
            return entradas.Select(MapToDto);
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetByHumorAsync(string humor)
        {
            var entradas = await _diarioEntradas.GetByHumorAsync(humor);
            return entradas.Select(MapToDto);
        }

        public async Task<ServiceResult<DiarioEntradaResponseDto>> CreateAsync(CreateDiarioEntradaDto dto)
        {
            if (!await _pets.ExistsAsync(dto.PetId))
                return ServiceResult<DiarioEntradaResponseDto>.Failure("Pet não encontrado.");

            var data = dto.Data.Date;
            if (await _diarioEntradas.ExistsByPetAndDateAsync(dto.PetId, data))
                return ServiceResult<DiarioEntradaResponseDto>.Failure(
                    "Já existe uma entrada para este pet nesta data.");

            var entrada = new DiarioEntrada
            {
                PetId = dto.PetId,
                Data = data,
                Resumo = dto.Resumo,
                HumorGeral = dto.HumorGeral,
                Status = dto.Status,
                CriadoEm = DateTime.Now
            };

            await _diarioEntradas.AddAsync(entrada);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<DiarioEntradaResponseDto>.Success(MapToDto(entrada));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateDiarioEntradaDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _diarioEntradas.GetByIdAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Entrada não encontrada.");

            var novaData = dto.Data.Date;
            if (existing.Data.Date != novaData &&
                await _diarioEntradas.ExistsByPetAndDateAsync(existing.PetId, novaData, excludeId: id))
            {
                return ServiceResult.Failure(
                    "Já existe uma entrada para este pet nesta data.");
            }

            existing.Data = novaData;
            existing.Resumo = dto.Resumo;
            existing.HumorGeral = dto.HumorGeral;
            existing.Status = dto.Status;
            existing.AtualizadoEm = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var entrada = await _diarioEntradas.GetByIdAsync(id);
            if (entrada == null)
                return ServiceResult.AsNotFound("Entrada não encontrada.");

            _diarioEntradas.Remove(entrada);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        private static DiarioEntradaResponseDto MapToDto(DiarioEntrada d) => new()
        {
            Id = d.Id,
            Data = d.Data,
            CriadoEm = d.CriadoEm,
            AtualizadoEm = d.AtualizadoEm,
            Resumo = d.Resumo,
            HumorGeral = d.HumorGeral,
            Status = d.Status,
            PetId = d.PetId
        };
    }
}
