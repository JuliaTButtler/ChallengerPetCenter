using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Registros;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class RegistroService : IRegistroService
    {
        private readonly IRegistroRepository _registros;
        private readonly IDiarioEntradaRepository _diarioEntradas;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _audit;

        public RegistroService(
            IRegistroRepository registros,
            IDiarioEntradaRepository diarioEntradas,
            IUnitOfWork unitOfWork,
            IAuditLogService audit)
        {
            _registros = registros;
            _diarioEntradas = diarioEntradas;
            _unitOfWork = unitOfWork;
            _audit = audit;
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetAllAsync()
        {
            var registros = await _registros.GetAllAsync();
            return registros.Select(MapToDto);
        }

        public async Task<ServiceResult<RegistroResponseDto>> GetByIdAsync(int id)
        {
            var registro = await _registros.GetByIdAsync(id);
            if (registro == null)
                return ServiceResult<RegistroResponseDto>.AsNotFound("Registro não encontrado.");

            return ServiceResult<RegistroResponseDto>.Success(MapToDto(registro));
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetByDiarioAsync(int diarioEntradaId)
        {
            var registros = await _registros.GetByDiarioAsync(diarioEntradaId);
            return registros.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetByTipoAsync(string tipo)
        {
            var registros = await _registros.GetByTipoAsync(tipo);
            return registros.Select(MapToDto);
        }

        public async Task<IEnumerable<RegistroResponseDto>> GetBySubtipoAsync(string subtipo)
        {
            var registros = await _registros.GetBySubtipoAsync(subtipo);
            return registros.Select(MapToDto);
        }

        public async Task<ServiceResult<RegistroResponseDto>> CreateAsync(CreateRegistroDto dto)
        {
            if (!await _diarioEntradas.ExistsAsync(dto.DiarioEntradaId))
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

            await _registros.AddAsync(registro);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "REGISTRO_CREATED",
                "Registro",
                registro.Id.ToString(),
                new { registro.Tipo, registro.DiarioEntradaId });

            return ServiceResult<RegistroResponseDto>.Success(MapToDto(registro));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateRegistroDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _registros.GetByIdAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Registro não encontrado.");

            existing.Tipo = dto.Tipo;
            existing.Subtipo = dto.Subtipo;
            existing.Valor = dto.Valor;
            existing.Unidade = dto.Unidade;
            existing.Nota = dto.Nota;
            existing.Horario = dto.Horario;
            existing.AtualizadoEm = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "REGISTRO_UPDATED",
                "Registro",
                id.ToString(),
                new { dto.Tipo });
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var registro = await _registros.GetByIdAsync(id);
            if (registro == null)
                return ServiceResult.AsNotFound("Registro não encontrado.");

            _registros.Remove(registro);
            await _unitOfWork.SaveChangesAsync();

            await _audit.WriteAsync(
                "REGISTRO_DELETED",
                "Registro",
                id.ToString(),
                new { registro.Tipo });
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
