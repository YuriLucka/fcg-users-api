using Users.Domain.Entities;
using Users.Domain.Enums;
using Users.Domain.Exceptions;

namespace Users.Tests.Domain
{
    /// <summary>
    /// Testes da entidade User seguindo TDD.
    /// Cada teste valida uma regra de negócio do domínio.
    /// </summary>
    public class UserEntityTests
    {
        // ── Criação válida ──────────────────────────────────────────────────────

        [Fact]
        public void User_DeveSerCriado_QuandoDadosValidos()
        {
            var user = new User("Teste", "teste@fiap.com", "hash_valido");

            Assert.Equal("Teste", user.Name);
            Assert.Equal("teste@fiap.com", user.Email.Value);
            Assert.Equal(UserRole.User, user.Role);
            Assert.NotEqual(Guid.Empty, user.Id);
        }

        [Fact]
        public void User_DeveTerRoleAdmin_QuandoCriadoComoAdmin()
        {
            var user = new User("Admin", "admin@fiap.com", "hash_valido", UserRole.Admin);

            Assert.Equal(UserRole.Admin, user.Role);
        }

        // ── Validação de nome ───────────────────────────────────────────────────

        [Fact]
        public void User_DeveLancarExcecao_QuandoNomeVazio()
        {
            var ex = Assert.Throws<DomainException>(() =>
                new User("", "teste@fiap.com", "hash_valido"));

            Assert.Equal("Nome é obrigatório.", ex.Message);
        }

        [Fact]
        public void User_DeveLancarExcecao_QuandoNomeNulo()
        {
            Assert.Throws<DomainException>(() =>
                new User(null!, "teste@fiap.com", "hash_valido"));
        }

        // ── Validação de e-mail ─────────────────────────────────────────────────

        [Theory]
        [InlineData("emailsemaroba")]
        [InlineData("email@")]
        [InlineData("@dominio.com")]
        [InlineData("")]
        public void User_DeveLancarExcecao_QuandoEmailInvalido(string emailInvalido)
        {
            Assert.Throws<DomainException>(() =>
                new User("Teste", emailInvalido, "hash_valido"));
        }

        [Fact]
        public void User_DeveArmazenarEmail_EmMinusculo()
        {
            var user = new User("Teste", "TESTE@FIAP.COM", "hash_valido");

            Assert.Equal("teste@fiap.com", user.Email.Value);
        }

        // ── Validação de senha (texto puro) ─────────────────────────────────────

        [Fact]
        public void ValidateRawPassword_DeveLancarExcecao_QuandoMenosDe8Caracteres()
        {
            var ex = Assert.Throws<DomainException>(() =>
                User.ValidateRawPassword("Ab@1"));

            Assert.Equal("Senha deve ter no mínimo 8 caracteres.", ex.Message);
        }

        [Fact]
        public void ValidateRawPassword_DeveLancarExcecao_QuandoSemNumeros()
        {
            var ex = Assert.Throws<DomainException>(() =>
                User.ValidateRawPassword("SenhaS@em"));

            Assert.Equal("Senha deve conter números.", ex.Message);
        }

        [Fact]
        public void ValidateRawPassword_DeveLancarExcecao_QuandoSemLetras()
        {
            var ex = Assert.Throws<DomainException>(() =>
                User.ValidateRawPassword("12345678@"));

            Assert.Equal("Senha deve conter letras.", ex.Message);
        }

        [Fact]
        public void ValidateRawPassword_DeveLancarExcecao_QuandoSemCaracterEspecial()
        {
            var ex = Assert.Throws<DomainException>(() =>
                User.ValidateRawPassword("Senha1234"));

            Assert.Equal("Senha deve conter caracteres especiais.", ex.Message);
        }

        [Fact]
        public void ValidateRawPassword_NaoDeveLancarExcecao_QuandoSenhaValida()
        {
            var exception = Record.Exception(() =>
                User.ValidateRawPassword("Senha@123"));

            Assert.Null(exception);
        }

        // ── Promoção de role ────────────────────────────────────────────────────

        [Fact]
        public void User_DeveSerPromovido_ParaAdmin()
        {
            var user = new User("Teste", "teste@fiap.com", "hash_valido");

            user.PromoteToAdmin();

            Assert.Equal(UserRole.Admin, user.Role);
        }

        [Fact]
        public void User_DeveSerRebaixado_ParaUser()
        {
            var user = new User("Teste", "teste@fiap.com", "hash_valido", UserRole.Admin);

            user.DemoteToUser();

            Assert.Equal(UserRole.User, user.Role);
        }
    }
}
