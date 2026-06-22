using Users.Domain.Enums;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // EF Core constructor
        protected User() { }

        public User(string name, string email, string passwordHash, UserRole role = UserRole.User)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Nome é obrigatório.");

            ValidatePasswordHash(passwordHash);

            Id = Guid.NewGuid();
            Name = name;
            Email = new Email(email);
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }

        public void PromoteToAdmin() => Role = UserRole.Admin;
        public void DemoteToUser() => Role = UserRole.User;

        public void UpdateProfile(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Nome é obrigatório.");

            Name = name;
            Email = new Email(email);
        }

        public static void ValidateRawPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new DomainException("Senha deve ter no mínimo 8 caracteres.");

            if (!Regex.IsMatch(password, @"[A-Za-z]"))
                throw new DomainException("Senha deve conter letras.");

            if (!Regex.IsMatch(password, @"\d"))
                throw new DomainException("Senha deve conter números.");

            if (!Regex.IsMatch(password, @"[^A-Za-z0-9]"))
                throw new DomainException("Senha deve conter caracteres especiais.");
        }

        private static void ValidatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Hash de senha não pode ser vazio.");
        }
    }
}
