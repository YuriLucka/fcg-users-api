using Users.Application.DTOs;
using Users.Application.Interfaces;
using Users.Domain.Exceptions;
using Users.Domain.Interfaces;

namespace Users.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<TokenDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email)
                ?? throw new DomainException("Credenciais inválidas.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordValid)
                throw new DomainException("Credenciais inválidas.");

            var token = _jwtTokenService.GenerateToken(user);

            return token;
        }
    }
}
