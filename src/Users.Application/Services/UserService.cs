using Fcg.Contracts.Events;
using MassTransit;
using Users.Application.DTOs;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Interfaces;

namespace Users.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPublishEndpoint _publish;

        public UserService(IUserRepository userRepository, IPublishEndpoint publish)
        {
            _userRepository = userRepository;
            _publish = publish;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new DomainException($"Usuário com Id '{id}' não encontrado.");

            return MapToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing is not null)
                throw new DomainException("Já existe um usuário com este e-mail.");

            User.ValidateRawPassword(dto.Password);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User(dto.Name, dto.Email, passwordHash);

            await _userRepository.AddAsync(user);

            await _publish.Publish(new UserCreatedEvent(user.Id, user.Name, user.Email.Value));

            return MapToDto(user);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new DomainException($"Usuário com Id '{id}' não encontrado.");

            var emailOwner = await _userRepository.GetByEmailAsync(dto.Email);
            if (emailOwner is not null && emailOwner.Id != id)
                throw new DomainException("Este e-mail já está em uso por outro usuário.");

            user.UpdateProfile(dto.Name, dto.Email);
            await _userRepository.UpdateAsync(user);

            return MapToDto(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new DomainException($"Usuário com Id '{id}' não encontrado.");

            await _userRepository.DeleteAsync(user.Id);
        }

        public async Task PromoteToAdminAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new DomainException($"Usuário com Id '{id}' não encontrado.");

            user.PromoteToAdmin();
            await _userRepository.UpdateAsync(user);
        }

        private static UserDto MapToDto(User user) => new(
            user.Id,
            user.Name,
            user.Email.Value,
            user.Role,
            user.CreatedAt
        );
    }
}
