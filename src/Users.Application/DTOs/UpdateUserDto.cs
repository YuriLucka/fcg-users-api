namespace Users.Application.DTOs
{
    /// <summary>
    /// Dados para atualização de um usuário existente.
    /// </summary>
    public record UpdateUserDto(
       /// <summary>Nome completo do usuário.</summary>
       string Name,
       /// <summary>Email do usuário.</summary>
       string Email
   );
}
