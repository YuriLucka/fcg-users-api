using Users.Domain.Enums;

namespace Users.Application.DTOs
{
    /// <summary>
    /// Dados de retorno de um usuário da plataforma.
    /// </summary>
    public record UserDto(
        /// <summary>Identificador único do usuário.</summary>
        Guid Id,
        /// <summary>Nome completo do usuário.</summary>
        string Name,
        /// <summary>Email do usuário.</summary>
        string Email,
        /// <summary>Papel do usuário (User/Admin).</summary>
        UserRole Role,
        /// <summary>Data de criação do usuário.</summary>
        DateTime CreatedAt
    );
}
