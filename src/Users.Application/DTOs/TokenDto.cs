namespace Users.Application.DTOs
{
    /// <summary>
    /// Dados do token JWT retornado após autenticação.
    /// </summary>
    public record TokenDto(
        /// <summary>Token de acesso JWT.</summary>
        string AccessToken,
        /// <summary>Tipo do token (ex: Bearer).</summary>
        string TokenType,
        /// <summary>Data e hora de expiração do token.</summary>
        DateTime ExpiresAt
    );
}
