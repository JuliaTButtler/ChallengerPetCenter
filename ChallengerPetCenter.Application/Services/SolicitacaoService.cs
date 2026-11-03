using ChallengerPetCenter.Abstractions;
using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Solicitacoes;
using ChallengerPetCenter.Models;
using ChallengerPetCenter.Repositories;

namespace ChallengerPetCenter.Services
{
    public class SolicitacaoService : ISolicitacaoService
    {
        private readonly ISolicitacaoRepository _solicitacoes;
        private readonly IPetRepository _pets;
        private readonly IUserRepository _users;
        private readonly IVeterinarioRepository _veterinarios;
        private readonly IUnitOfWork _unitOfWork;

        public SolicitacaoService(
            ISolicitacaoRepository solicitacoes,
            IPetRepository pets,
            IUserRepository users,
            IVeterinarioRepository veterinarios,
            IUnitOfWork unitOfWork)
        {
            _solicitacoes = solicitacoes;
            _pets = pets;
            _users = users;
            _veterinarios = veterinarios;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetAllAsync()
        {
            var solicitacoes = await _solicitacoes.GetAllAsync();
            return solicitacoes.Select(MapToDto);
        }

        public async Task<ServiceResult<SolicitacaoResponseDto>> GetByIdAsync(int id)
        {
            var solicitacao = await _solicitacoes.GetByIdAsync(id);
            if (solicitacao == null)
                return ServiceResult<SolicitacaoResponseDto>.AsNotFound("Solicitação não encontrada.");

            return ServiceResult<SolicitacaoResponseDto>.Success(MapToDto(solicitacao));
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByPetAsync(int petId)
        {
            var solicitacoes = await _solicitacoes.GetByPetAsync(petId);
            return solicitacoes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByTutorAsync(int tutorId)
        {
            var solicitacoes = await _solicitacoes.GetByTutorAsync(tutorId);
            return solicitacoes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByVeterinarioAsync(int veterinarioId)
        {
            var solicitacoes = await _solicitacoes.GetByVeterinarioAsync(veterinarioId);
            return solicitacoes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoResponseDto>> GetByStatusAsync(string status)
        {
            var solicitacoes = await _solicitacoes.GetByStatusAsync(status);
            return solicitacoes.Select(MapToDto);
        }

        public async Task<ServiceResult<SolicitacaoResponseDto>> CreateAsync(CreateSolicitacaoDto dto)
        {
            var pet = await _pets.GetByIdAsync(dto.PetId);
            if (pet == null)
                return ServiceResult<SolicitacaoResponseDto>.Failure("Pet não encontrado.");

            if (!await _users.ExistsAsync(dto.TutorId))
                return ServiceResult<SolicitacaoResponseDto>.Failure("Tutor não encontrado.");

            if (pet.UserId != dto.TutorId)
                return ServiceResult<SolicitacaoResponseDto>.Failure(
                    "O tutor informado não é o responsável por este pet.");

            if (!await _veterinarios.ExistsAsync(dto.VeterinarioId))
                return ServiceResult<SolicitacaoResponseDto>.Failure("Veterinário não encontrado.");

            var status = string.IsNullOrWhiteSpace(dto.Status)
                ? "PENDENTE"
                : dto.Status.ToUpper();

            var solicitacao = new Solicitacao
            {
                Status = status,
                Mensagem = dto.Mensagem,
                PetId = dto.PetId,
                TutorId = dto.TutorId,
                VeterinarioId = dto.VeterinarioId,
                CriadoEm = DateTime.Now
            };

            await _solicitacoes.AddAsync(solicitacao);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<SolicitacaoResponseDto>.Success(MapToDto(solicitacao));
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateSolicitacaoDto dto)
        {
            if (id != dto.Id)
                return ServiceResult.Failure("ID da rota diferente do ID enviado.");

            var existing = await _solicitacoes.GetByIdAsync(id);
            if (existing == null)
                return ServiceResult.AsNotFound("Solicitação não encontrada.");

            var novoStatus = dto.Status.ToUpper();
            var statusAnterior = existing.Status.ToUpper();

            existing.Status = novoStatus;
            existing.Mensagem = dto.Mensagem;

            if (dto.RespondidoEm.HasValue)
            {
                existing.RespondidoEm = dto.RespondidoEm;
            }
            else if (statusAnterior == "PENDENTE" &&
                     (novoStatus == "ACEITA" || novoStatus == "RECUSADA" ||
                      novoStatus == "ACEITO" || novoStatus == "RECUSADO"))
            {
                existing.RespondidoEm = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var solicitacao = await _solicitacoes.GetByIdAsync(id);
            if (solicitacao == null)
                return ServiceResult.AsNotFound("Solicitação não encontrada.");

            _solicitacoes.Remove(solicitacao);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        private static SolicitacaoResponseDto MapToDto(Solicitacao s) => new()
        {
            Id = s.Id,
            Status = s.Status,
            Mensagem = s.Mensagem,
            CriadoEm = s.CriadoEm,
            RespondidoEm = s.RespondidoEm,
            PetId = s.PetId,
            TutorId = s.TutorId,
            VeterinarioId = s.VeterinarioId
        };
    }
}
