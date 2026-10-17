using Microsoft.Extensions.Configuration;
using Users.Infrastructure.Security;

namespace Users.Tests.Infrastructure
{
    public class JwtTokenServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateToken_DeveLancarExcecao_QuandoSecretKeyAusenteOuVazia(string? secret)
        {
            var settings = new Dictionary<string, string?> { ["JwtSettings:SecretKey"] = secret };
            var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var service = new JwtTokenService(config);

            var ex = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(null!));
            Assert.Contains("SecretKey", ex.Message);
        }
    }
}
