namespace Users.Application.DTOs
{
    /// <summary>
    /// Dados para login do usuário.
    /// </summary>
    public record LoginDto(
        /// <summary>Email do usuário.</summary>
        string Email,
        /// <summary>Senha do usuário.</summary>
        string Password
    );
}
