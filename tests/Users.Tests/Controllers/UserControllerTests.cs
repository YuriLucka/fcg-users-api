using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Users.API.Controllers;
using Users.Application.DTOs;
using Users.Application.Interfaces;
using Users.Domain.Enums;

namespace Users.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        private static void SetUser(UserController controller, Guid userId, string role = "")
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // ── GetById ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_DeveRetornarForbid_QuandoNaoForDonoNemAdmin()
        {
            var donoId = Guid.NewGuid();
            var chamadorId = Guid.NewGuid();
            SetUser(_controller, chamadorId);

            var result = await _controller.GetById(donoId);

            Assert.IsType<ForbidResult>(result);
            _userServiceMock.Verify(s => s.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetById_DeveRetornarOk_QuandoForODono()
        {
            var donoId = Guid.NewGuid();
            SetUser(_controller, donoId);
            _userServiceMock.Setup(s => s.GetByIdAsync(donoId))
                .ReturnsAsync(new UserDto(donoId, "Dono", "dono@fcg.com", UserRole.User, DateTime.UtcNow));

            var result = await _controller.GetById(donoId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_DeveRetornarOk_QuandoForAdmin()
        {
            var donoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            SetUser(_controller, adminId, role: "Admin");
            _userServiceMock.Setup(s => s.GetByIdAsync(donoId))
                .ReturnsAsync(new UserDto(donoId, "Dono", "dono@fcg.com", UserRole.User, DateTime.UtcNow));

            var result = await _controller.GetById(donoId);

            Assert.IsType<OkObjectResult>(result);
        }

        // ── Update ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_DeveRetornarForbid_QuandoNaoForDonoNemAdmin()
        {
            var donoId = Guid.NewGuid();
            var chamadorId = Guid.NewGuid();
            SetUser(_controller, chamadorId);
            var dto = new UpdateUserDto("Hack", "hack@fcg.com");

            var result = await _controller.Update(donoId, dto);

            Assert.IsType<ForbidResult>(result);
            _userServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateUserDto>()), Times.Never);
        }

        [Fact]
        public async Task Update_DeveRetornarOk_QuandoForODono()
        {
            var donoId = Guid.NewGuid();
            SetUser(_controller, donoId);
            var dto = new UpdateUserDto("Novo Nome", "dono@fcg.com");
            _userServiceMock.Setup(s => s.UpdateAsync(donoId, dto))
                .ReturnsAsync(new UserDto(donoId, "Novo Nome", "dono@fcg.com", UserRole.User, DateTime.UtcNow));

            var result = await _controller.Update(donoId, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_DeveRetornarOk_QuandoForAdmin()
        {
            var donoId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            SetUser(_controller, adminId, role: "Admin");
            var dto = new UpdateUserDto("Novo Nome", "dono@fcg.com");
            _userServiceMock.Setup(s => s.UpdateAsync(donoId, dto))
                .ReturnsAsync(new UserDto(donoId, "Novo Nome", "dono@fcg.com", UserRole.User, DateTime.UtcNow));

            var result = await _controller.Update(donoId, dto);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
