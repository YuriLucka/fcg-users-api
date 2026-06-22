using Users.Application.DTOs;
using Users.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Cria um novo usuário na plataforma.
        /// </summary>
        /// <remarks>
        /// Requisição anônima. Retorna o usuário criado e a URL para consulta.
        /// </remarks>
        /// <param name="dto">Dados para criação do usuário (nome, email, senha).</param>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Dados inválidos ou usuário já existente.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Retorna os dados de um usuário pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="200">Usuário encontrado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Lista todos os usuários cadastrados. Apenas administradores podem acessar.
        /// </summary>
        /// <response code="200">Lista de usuários retornada com sucesso.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <param name="dto">Novos dados do usuário (nome, email).</param>
        /// <response code="200">Usuário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userService.UpdateAsync(id, dto);
            return Ok(user);
        }

        /// <summary>
        /// Remove um usuário do sistema. Apenas administradores podem remover usuários.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="204">Usuário removido com sucesso.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Promove um usuário para o papel de administrador. Apenas administradores podem promover.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="204">Usuário promovido com sucesso.</response>
        [HttpPatch("{id:guid}/promote")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Promote(Guid id)
        {
            await _userService.PromoteToAdminAsync(id);
            return NoContent();
        }
    }
}
