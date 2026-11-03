using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface ISolicitacaoRepository
    {
        Task<IReadOnlyList<Solicitacao>> GetAllAsync();
        Task<Solicitacao?> GetByIdAsync(int id);
        Task<IReadOnlyList<Solicitacao>> GetByPetAsync(int petId);
        Task<IReadOnlyList<Solicitacao>> GetByTutorAsync(int tutorId);
        Task<IReadOnlyList<Solicitacao>> GetByVeterinarioAsync(int veterinarioId);
        Task<IReadOnlyList<Solicitacao>> GetByStatusAsync(string status);
        Task AddAsync(Solicitacao solicitacao);
        void Remove(Solicitacao solicitacao);
    }
}
