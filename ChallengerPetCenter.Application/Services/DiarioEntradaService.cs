using ChallengerPetCenter.Common;
using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.DTOs.DiarioEntradas;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class DiarioEntradaService : IDiarioEntradaService
    {
        private readonly IAppDbContext _context;

        public DiarioEntradaService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetAllAsync()
        {
            var entradas = await _context.DiarioEntradas.ToListAsync();
            return entradas.Select(MapToDto);
        }

        public async Task<ServiceResult<DiarioEntradaResponseDto>> GetByIdAsync(int id)
        {
            var entrada = await _context.DiarioEntradas.FindAsync(id);
            if (entrada == null)
                return ServiceResult<DiarioEntradaResponseDto>.AsNotFound("Entrada não encontrada.");

            return ServiceResult<DiarioEntradaResponseDto>.Success(MapToDto(entrada));
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetByPetAsync(int petId)
        {
            var entradas = await _context.DiarioEntradas
                .Where(d => d.PetId == petId)
                .OrderByDescending(d => d.Data)
                .ToListAsync();

            return entradas.Select(MapToDto);
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetByStatusAsync(string status)
        {
            var entradas = await _context.DiarioEntradas
                .Where(d =>
                    d.Status != null &&
                    d.Status.ToUpper() == status.ToUpper())
                .ToListAsync();

            return entradas.Select(MapToDto);
        }

        public async Task<IEnumerable<DiarioEntradaResponseDto>> GetByHumorAsync(string humor)
        {
            var entradas = await _context.DiarioEntradas
                .Where(d =>
                    d.HumorGeral != null &&
                    d.HumorGeral.ToUpper() == humor.ToUpper())
                .ToListAsync();

            return entradas.Select(MapToDto);
        }

        public async Task<ServiceResult<DiarioEntradaResponseDto>> CreateAsync(CreateDiarioEntradaDto dto)
        {
            var petExiste = await _context.Pets.CountAsync(p => p.Id == dto.PetId) > 0;
            if (!petExiste)
                return ServiceResult<DiarioEntradaResponseDto>.Failure("Pet não encontrado.");

            var data = dto.Data.Date;
            var entradaExiste = await _context.DiarioEntradas
                .CountAsync(d => d.PetId == dto.PetId && d.Data.Date == data) > 0;

            if (entradaExiste)
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

            _context.DiarioEntradas.Add(entrada);
            await _context.SaveChangesAsync();

            return ServiceResult<DiarioEntradaResponseDto>.Success(MapToDto(entrada));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateDiarioEntradaDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _context.DiarioEntradas.FindAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Entrada não encontrada.");

            var novaData = dto.Data.Date;
            if (existing.Data.Date != novaData)
            {
                var conflito = await _context.DiarioEntradas
                    .CountAsync(d =>
                        d.Id != id &&
                        d.PetId == existing.PetId &&
                        d.Data.Date == novaData) > 0;

                if (conflito)
                    return ServiceResult.Failure(
                        "Já existe uma entrada para este pet nesta data.");
            }

            existing.Data = novaData;
            existing.Resumo = dto.Resumo;
            existing.HumorGeral = dto.HumorGeral;
            existing.Status = dto.Status;
            existing.AtualizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var entrada = await _context.DiarioEntradas.FindAsync(id);
            if (entrada == null)
                return ServiceResult.AsNotFound("Entrada não encontrada.");

            _context.DiarioEntradas.Remove(entrada);
            await _context.SaveChangesAsync();
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
