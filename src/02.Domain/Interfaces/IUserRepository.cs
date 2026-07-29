using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsWithRoleAsync(Guid userId, UserRole role);
    Task<IEnumerable<User>> GetAllAsync(); // REQ-4.1
    Task<User> AddAsync(User user);        // REQ-4.2
    Task UpdateAsync(User user);           // REQ-4.3, REQ-4.4
}