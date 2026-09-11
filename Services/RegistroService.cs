using ChallengerPetCenter.Common;
using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class RegistroService : IRegistroService
    {
        private readonly AppDbContext _context;

        public RegistroService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetAllAsync()
        {
            var registros = await _context.Registros.ToListAsync();
            return registros.Select(MapToDto);
        }

        public async Task<ServiceResult<RegistroResponseDto>> GetByIdAsync(int id)
        {
            var registro = await _context.Registros.FindAsync(id);
            if (registro == null)
                return ServiceResult<RegistroResponseDto>.AsNotFound("Registro não encontrado.");

            return ServiceResult<RegistroResponseDto>.Success(MapToDto(registro));
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetByDiarioAsync(int diarioEntradaId)
        {
            var registros = await _context.Registros
                .Where(r => r.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

            return registros.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetByTipoAsync(string tipo)
        {
            var registros = await _context.Registros
                .Where(r => r.Tipo.ToUpper() == tipo.ToUpper())
                .ToListAsync();

            return registros.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetBySubtipoAsync(string subtipo)
        {
            var registros = await _context.Registros
                .Where(r =>
                    r.Subtipo != null &&
                    r.Subtipo.ToUpper() == subtipo.ToUpper())
                .ToListAsync();

            return registros.Select(MapToDto);
        }

        public async Task<ServiceResult<RegistroResponseDto>> CreateAsync(CreateRegistroDto dto)
        {
            var diarioExiste = await _context.DiarioEntradas
                .CountAsync(d => d.Id == dto.DiarioEntradaId) > 0;

            if (!diarioExiste)
                return ServiceResult<RegistroResponseDto>.Failure("Entrada do diário não encontrada.");

            var registro = new Registro
            {
                Tipo = dto.Tipo,
                Subtipo = dto.Subtipo,
                Valor = dto.Valor,
                Unidade = dto.Unidade,
                Nota = dto.Nota,
                Horario = dto.Horario,
                DiarioEntradaId = dto.DiarioEntradaId
            };

            _context.Registros.Add(registro);
            await _context.SaveChangesAsync();

            return ServiceResult<RegistroResponseDto>.Success(MapToDto(registro));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateRegistroDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _context.Registros.FindAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Registro não encontrado.");

            existing.Tipo = dto.Tipo;
            existing.Subtipo = dto.Subtipo;
            existing.Valor = dto.Valor;
            existing.Unidade = dto.Unidade;
            existing.Nota = dto.Nota;
            existing.Horario = dto.Horario;
            existing.AtualizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var registro = await _context.Registros.FindAsync(id);
            if (registro == null)
                return ServiceResult.AsNotFound("Registro não encontrado.");

            _context.Registros.Remove(registro);
            await _context.SaveChangesAsync();
            return ServiceResult.Success();
        }

        private static RegistroResponseDto MapToDto(Registro r) => new()
        {
            Id = r.Id,
            Tipo = r.Tipo,
            Subtipo = r.Subtipo,
            Valor = r.Valor,
            Unidade = r.Unidade,
            Nota = r.Nota,
            Horario = r.Horario,
            AtualizadoEm = r.AtualizadoEm,
            DiarioEntradaId = r.DiarioEntradaId
        };
    }
}
