using MassTransit;
using Moq;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain.Entities;
using Users.Domain.Enums;
using Users.Domain.Exceptions;
using Users.Domain.Interfaces;

namespace Users.Tests.Application
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPublishEndpoint> _publishMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _publishMock = new Mock<IPublishEndpoint>();
            _userService = new UserService(_userRepoMock.Object, _publishMock.Object);
        }

        // ── GetById ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_DeveRetornarUser_QuandoExistir()
        {
            var user = new User("Teste", "teste@fiap.com", "hash");
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _userService.GetByIdAsync(user.Id);

            Assert.Equal(user.Id, result.Id);
            Assert.Equal("teste@fiap.com", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_DeveLancarExcecao_QuandoNaoExistir()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<DomainException>(() =>
                _userService.GetByIdAsync(Guid.NewGuid()));
        }

        // ── CreateAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_DeveCriarUser_QuandoDadosValidos()
        {
            var dto = new CreateUserDto("Teste", "teste@fiap.com", "Senha@123");

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _userService.CreateAsync(dto);

            Assert.Equal("Teste", result.Name);
            Assert.Equal("teste@fiap.com", result.Email);
            Assert.Equal(UserRole.User, result.Role);

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarExcecao_QuandoEmailJaExistir()
        {
            var dto = new CreateUserDto("Teste", "teste@fiap.com", "Senha@123");
            var existingUser = new User("Outro", "teste@fiap.com", "hash");

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existingUser);

            var ex = await Assert.ThrowsAsync<DomainException>(() =>
                _userService.CreateAsync(dto));

            Assert.Equal("Já existe um usuário com este e-mail.", ex.Message);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("semnum@fiap.com", "SenhaSemNum@")]
        [InlineData("semletra@fiap.com", "12345678@")]
        [InlineData("semespecial@fiap.com", "Senha1234")]
        [InlineData("curta@fiap.com", "Ab@1")]
        public async Task CreateAsync_DeveLancarExcecao_QuandoSenhaInvalida(string email, string senha)
        {
            var dto = new CreateUserDto("Teste", email, senha);
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<DomainException>(() =>
                _userService.CreateAsync(dto));

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        // ── DeleteAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_DeveDeletar_QuandoUserExistir()
        {
            var user = new User("Teste", "teste@fiap.com", "hash");
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.DeleteAsync(user.Id)).Returns(Task.CompletedTask);

            await _userService.DeleteAsync(user.Id);

            _userRepoMock.Verify(r => r.DeleteAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveLancarExcecao_QuandoUserNaoExistir()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<DomainException>(() =>
                _userService.DeleteAsync(Guid.NewGuid()));
        }

        // ── UpdateAsync ─────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_DeveAtualizarNomeEEmail_QuandoDadosValidos()
        {
            var user = new User("Nome Antigo", "antigo@fiap.com", "hash");
            var dto = new UpdateUserDto("Nome Novo", "novo@fiap.com");

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _userService.UpdateAsync(user.Id, dto);

            Assert.Equal("Nome Novo", result.Name);
            Assert.Equal("novo@fiap.com", result.Email);
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarExcecao_QuandoEmailEmUsoPorOutroUsuario()
        {
            var user = new User("Nome", "original@fiap.com", "hash");
            var other = new User("Outro", "ocupado@fiap.com", "hash");
            var dto = new UpdateUserDto("Nome", "ocupado@fiap.com");

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(other);

            await Assert.ThrowsAsync<DomainException>(() =>
                _userService.UpdateAsync(user.Id, dto));

            _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        // ── PromoteToAdmin ──────────────────────────────────────────────────────

        [Fact]
        public async Task PromoteToAdminAsync_DevePromover_QuandoUserExistir()
        {
            var user = new User("Teste", "teste@fiap.com", "hash");
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            await _userService.PromoteToAdminAsync(user.Id);

            Assert.Equal(UserRole.Admin, user.Role);
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
