using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Auth;

namespace TicketManagement.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration,
    ISystemLogService systemLogService)
    : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email)
            ?? throw new ValidationException("Email", "Email atau password salah.");

        // NOTE: password hashing pakai BCrypt (tech.md Section 4.3).
        // Perlu tambahkan package BCrypt.Net-Next di 04.Application.
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new ValidationException("Email", "Email atau password salah.");

        var (token, expiresAt) = GenerateJwtToken(user.Id, user.Name, user.Role.ToString());

        await systemLogService.LogAsync(user.Id, SystemLogAction.Login, "Berhasil Login ke sistem");

        return new LoginResponseDto
        {
            Token = token,
            Name = user.Name,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        };
    }

    private (string Token, DateTime ExpiresAt) GenerateJwtToken(Guid userId, string name, string role)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role)
        };

        var expiresAt = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}