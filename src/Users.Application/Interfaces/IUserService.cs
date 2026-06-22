using Users.Application.DTOs;

namespace Users.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task DeleteAsync(Guid id);
        Task PromoteToAdminAsync(Guid id);
    }
}
