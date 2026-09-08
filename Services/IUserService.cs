using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Users;

namespace ChallengerPetCenter.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email);
        Task<IEnumerable<UserResponseDto>> GetByTipoAsync(string tipo);
        Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateUserDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
