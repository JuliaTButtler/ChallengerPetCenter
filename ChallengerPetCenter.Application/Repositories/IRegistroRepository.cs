using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IRegistroRepository
    {
        Task<IReadOnlyList<Registro>> GetAllAsync();
        Task<Registro?> GetByIdAsync(int id);
        Task<IReadOnlyList<Registro>> GetByDiarioAsync(int diarioEntradaId);
        Task<IReadOnlyList<Registro>> GetByTipoAsync(string tipo);
        Task<IReadOnlyList<Registro>> GetBySubtipoAsync(string subtipo);
        Task AddAsync(Registro registro);
        void Remove(Registro registro);
    }
}
