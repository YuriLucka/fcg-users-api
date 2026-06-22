using Users.Application.DTOs;
using Users.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza o login do usuário e retorna o token JWT para autenticação.
        /// </summary>
        /// <param name="dto">Dados de login (email e senha).</param>
        /// <response code="200">Login realizado com sucesso.</response>
        /// <response code="400">Credenciais inválidas.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(token);
        }
    }
}
