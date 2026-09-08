using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Services.Common;

namespace ChallengerPetCenter.Services
{
    public interface IUserService
    {
        Task<ServiceResult<IEnumerable<UserResponseDto>>> GetAllAsync();
        Task<ServiceResult<UserResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<UserResponseDto>> GetByEmailAsync(string email);
        Task<ServiceResult<IEnumerable<UserResponseDto>>> GetByTipoAsync(string tipo);
        Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto);
        Task<ServiceResult> UpdateAsync(int id, UpdateUserDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
