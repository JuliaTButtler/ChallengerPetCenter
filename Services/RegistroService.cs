using ChallengerPetCenter.Data;
using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace ChallengerPetCenter.Services
{
    public class RegistroService : IRegistroService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RegistroService> _logger;

        public RegistroService(AppDbContext context, ILogger<RegistroService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetAllAsync()
        {
            var registros = await _context.Registros.AsNoTracking().ToListAsync();
            return ServiceResult<IEnumerable<RegistroResponseDto>>.Ok(registros.Select(MapToDto));
        }

        public async Task<ServiceResult<RegistroResponseDto>> GetByIdAsync(int id)
        {
            var registro = await _context.Registros.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registro is null)
            {
                return ServiceResult<RegistroResponseDto>.NotFound("Registro não encontrado.");
            }

            return ServiceResult<RegistroResponseDto>.Ok(MapToDto(registro));
        }

        public async Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetByDiarioAsync(int diarioEntradaId)
        {
            var registros = await _context.Registros.AsNoTracking()
                .Where(r => r.DiarioEntradaId == diarioEntradaId)
                .ToListAsync();

            if (registros.Count == 0)
            {
                return ServiceResult<IEnumerable<RegistroResponseDto>>.NotFound("Nenhum registro encontrado.");
            }

            return ServiceResult<IEnumerable<RegistroResponseDto>>.Ok(registros.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetByTipoAsync(string tipo)
        {
            var registros = await _context.Registros.AsNoTracking()
                .Where(r => r.Tipo.ToUpper() == tipo.ToUpper())
                .ToListAsync();

            if (registros.Count == 0)
            {
                return ServiceResult<IEnumerable<RegistroResponseDto>>.NotFound("Nenhum registro encontrado.");
            }

            return ServiceResult<IEnumerable<RegistroResponseDto>>.Ok(registros.Select(MapToDto));
        }

        public async Task<ServiceResult<IEnumerable<RegistroResponseDto>>> GetBySubtipoAsync(string subtipo)
        {
            var registros = await _context.Registros.AsNoTracking()
                .Where(r => r.Subtipo != null && r.Subtipo.ToUpper() == subtipo.ToUpper())
                .ToListAsync();

            if (registros.Count == 0)
            {
                return ServiceResult<IEnumerable<RegistroResponseDto>>.NotFound("Nenhum registro encontrado.");
            }

            return ServiceResult<IEnumerable<RegistroResponseDto>>.Ok(registros.Select(MapToDto));
        }

        public async Task<ServiceResult<RegistroResponseDto>> CreateAsync(CreateRegistroDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Tipo))
            {
                return ServiceResult<RegistroResponseDto>.BadRequest("Tipo é obrigatório.");
            }

            var diarioExists = await _context.DiarioEntradas.AnyAsync(d => d.Id == dto.DiarioEntradaId);
            if (!diarioExists)
            {
                return ServiceResult<RegistroResponseDto>.BadRequest("Entrada do diário não encontrada.");
            }

            var registro = new Registro
            {
                Tipo = dto.Tipo.Trim(),
                Subtipo = dto.Subtipo,
                Valor = dto.Valor,
                Unidade = dto.Unidade,
                Nota = dto.Nota,
                Horario = dto.Horario,
                DiarioEntradaId = dto.DiarioEntradaId
            };

            _context.Registros.Add(registro);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Registro {RegistroId} criado na entrada {DiarioEntradaId}", registro.Id, registro.DiarioEntradaId);
            return ServiceResult<RegistroResponseDto>.Ok(MapToDto(registro));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateRegistroDto dto)
        {
            if (id != dto.Id)
            {
                return ServiceResult.BadRequest("ID da rota diferente do ID enviado.");
            }

            var existing = await _context.Registros.FindAsync(id);
            if (existing is null)
            {
                return ServiceResult.NotFound("Registro não encontrado.");
            }

            existing.Tipo = dto.Tipo;
            existing.Subtipo = dto.Subtipo;
            existing.Valor = dto.Valor;
            existing.Unidade = dto.Unidade;
            existing.Nota = dto.Nota;
            existing.Horario = dto.Horario;
            existing.AtualizadoEm = DateTime.Now;

            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var registro = await _context.Registros.FindAsync(id);
            if (registro is null)
            {
                return ServiceResult.NotFound("Registro não encontrado.");
            }

            _context.Registros.Remove(registro);
            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        private static RegistroResponseDto MapToDto(Registro registro) => new()
        {
            Id = registro.Id,
            Tipo = registro.Tipo,
            Subtipo = registro.Subtipo,
            Valor = registro.Valor,
            Unidade = registro.Unidade,
            Nota = registro.Nota,
            Horario = registro.Horario,
            AtualizadoEm = registro.AtualizadoEm,
            DiarioEntradaId = registro.DiarioEntradaId
        };
    }
}
