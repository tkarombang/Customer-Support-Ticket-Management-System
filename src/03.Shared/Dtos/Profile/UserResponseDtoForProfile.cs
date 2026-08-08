namespace TicketManagement.Shared.Dtos.Profile
{
    // DTO kecil khusus tampilan profile diri sendiri
    public class UserResponseDtoForProfile
    {
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public required string Role { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
