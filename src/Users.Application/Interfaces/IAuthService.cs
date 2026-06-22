using Users.Application.DTOs;

namespace Users.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto dto);
    }
}
