using Users.Application.DTOs;
using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IJwtTokenService
    {
        TokenDto GenerateToken(User user);
    }
}
