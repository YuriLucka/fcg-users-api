using Fcg.Contracts.Events;
using MassTransit;
using Moq;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Xunit;

namespace Users.Tests;

public class PublishUserCreatedTests
{
    [Fact]
    public async Task CreateAsync_publishes_UserCreatedEvent()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var publish = new Mock<IPublishEndpoint>();
        var sut = new UserService(repo.Object, publish.Object);
        var dto = new CreateUserDto("Ana", "ana@fcg.com", "P@ssw0rd1");

        await sut.CreateAsync(dto);

        publish.Verify(p => p.Publish(
            It.Is<UserCreatedEvent>(e => e.Email == "ana@fcg.com" && e.Name == "Ana"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
