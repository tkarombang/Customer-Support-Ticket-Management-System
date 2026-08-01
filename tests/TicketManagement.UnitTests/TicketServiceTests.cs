using Moq;
using TicketManagement.Application.Services;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Tickets;
using Xunit;

namespace TicketManagement.UnitTests;

public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly TicketService _sut; // system under test

    public TicketServiceTests()
    {
        _sut = new TicketService(_ticketRepoMock.Object, _userRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateTicketNumber_WithCorrectFormat()
    {
        // Arrange
        _ticketRepoMock.Setup(r => r.GetNextTicketSequenceAsync()).ReturnsAsync(4);
        _ticketRepoMock.Setup(r => r.AddAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket t) => t); // echo back apa yang di-passing

        var dto = new CreateTicketDto
        {
            CustomerName = "Budi",
            CustomerEmail = "budi@example.com",
            Title = "Login gagal",
            Description = "Tidak bisa login sejak kemarin",
            Type = "Incident",
            Impact = "Tidak Valid",
            Category = "Test",
            Priority = "Hight"
        };

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        Assert.Equal("TKT-00005", result.TicketNumber);
        Assert.Equal("Open", result.Status); // REQ-2.3
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenTicketIsClosed()
    {
        // Arrange — REQ-2.5: Closed ticket tidak boleh dimodifikasi
        var closedTicket = new Ticket
        {
            Id = Guid.Parse("3454554-4444-233-34345345-12312312311"),
            TicketNumber = "TKT-00001",
            CustomerName = "Budi",
            CustomerEmail = "budi@example.com",
            Title = "Test",
            Description = "Test",
            Status = TicketStatus.Closed
        };
        _ticketRepoMock.Setup(r => r.GetByIdAsync(Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"))).ReturnsAsync(closedTicket);

        var dto = new UpdateTicketDto { Description = "Coba ubah", Status = "Open" };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"), dto, changedByUserId: Guid.Parse("99")));
    }

    [Fact]
    public async Task AssignAsync_ShouldThrow_WhenAssigneeIsNotSupportAgent()
    {
        // Arrange — REQ-2.6: assignee harus role SupportAgent
        var ticket = new Ticket
        {
            Id = Guid.Parse("E2222222-2222-2222-2222-222222222222"),
            TicketNumber = "TKT-00001",
            CustomerName = "Budi",
            CustomerEmail = "budi@example.com",
            Title = "Test",
            Description = "Test",
            Status = TicketStatus.Open
        };
        _ticketRepoMock.Setup(r => r.GetByIdAsync(Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"))).ReturnsAsync(ticket);
        _userRepoMock.Setup(r => r.ExistsWithRoleAsync(Guid.Parse("5"), UserRole.Agent)).ReturnsAsync(false);

        var dto = new AssignTicketDto { AssignedToUserId = Guid.Parse("5") };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.AssignAsync(Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"), dto, changedByUserId: Guid.Parse("99")));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFound_WhenTicketDoesNotExist()
    {
        _ticketRepoMock.Setup(r => r.GetByIdAsync(Guid.Parse("999"))).ReturnsAsync((Ticket?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(Guid.Parse("999")));
    }
}