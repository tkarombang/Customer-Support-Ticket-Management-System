using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Users;

namespace TicketManagement.Application.Services;

public class UserService(
    IUserRepository userRepository,
    ISystemLogService systemLogService
    ) : IUserService
{
    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto, Guid createdBy)
    {
        var existing = await userRepository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new ValidationException("Email", "Email sudah terdaftar.");

        if (!Enum.TryParse<UserRole>(dto.Role, out var role))
            throw new ValidationException("Role", $"Role '{dto.Role}' tidak valid.");

        var user = new User
        {
            Username = dto.Username,
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
            Status = UserStatus.Active
        };

        var created = await userRepository.AddAsync(user);

        await systemLogService.LogAsync(createdBy, SystemLogAction.CreateUser, "Berhasil membuat User");
        return MapToDto(created);
    }

    public async Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserDto dto, Guid updatedBy)
    {
        var user = await userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);

        if (!Enum.TryParse<UserRole>(dto.Role, out var role))
            throw new ValidationException("Role", $"Role '{dto.Role}' tidak valid.");

        user.Name = dto.Name;
        user.Role = role;
        user.PhoneNumber = dto.PhoneNumber;
        user.JobTitle = dto.JobTitle;
        user.Address = dto.Address;
        user.UpdatedDate = DateTime.UtcNow;

        await userRepository.UpdateAsync(user);

        await systemLogService.LogAsync(updatedBy, SystemLogAction.UpdateSettings, "Berhasil Memperbaharui User");
        return MapToDto(user);
    }

    public async Task<UserResponseDto> ToggleStatusAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);

        user.Status = user.Status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
        user.UpdatedDate = DateTime.UtcNow;

        await userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    private static UserResponseDto MapToDto(User user) => new()
    {
        UserId = user.Id,
        Username = user.Username,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.ToString(),
        Status = user.Status.ToString(),
        CreatedDate = user.CreatedDate
    };
}