using TicketManagement.Shared.Dtos.Users;

namespace TicketManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> CreateAsync(CreateUserDto dto);
        Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task<UserResponseDto> ToggleStatusAsync(Guid id);
    }
}
