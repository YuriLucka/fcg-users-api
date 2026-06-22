namespace Users.Application.DTOs
{
    /// <summary>
    /// Dados para criação de um novo usuário.
    /// </summary>
    public record CreateUserDto(
        /// <summary>Nome completo do usuário.</summary>
        string Name,
        /// <summary>Email do usuário.</summary>
        string Email,
        /// <summary>Senha do usuário.</summary>
        string Password
    );
}
