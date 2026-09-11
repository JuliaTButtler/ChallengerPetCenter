using ChallengerPetCenter.Common;
using ChallengerPetCenter.DTOs.Auth;

namespace ChallengerPetCenter.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
    }
}
