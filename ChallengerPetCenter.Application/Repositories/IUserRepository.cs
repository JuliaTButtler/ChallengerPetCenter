using ChallengerPetCenter.Models;

namespace ChallengerPetCenter.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IReadOnlyList<User>> GetByTipoAsync(string tipo);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByEmailExcludingIdAsync(string email, int excludeId);
        Task AddAsync(User user);
        void Remove(User user);
    }
}
