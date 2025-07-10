using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Implementations;

namespace UserManagement.Services.Tests;

public class LogServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private LogService CreateService() => new(_dataContext.Object);



    private static User TestUser => new()
    {
        Id = 1,
        Forename = "Test",
        Surname = "User",
        Email = "test@example.com",
        IsActive = true,
        DateOfBirth = new DateOnly(1990, 1, 1)
    };

    private List<Log> SetupLogs()
    {
        return new List<Log>
        {
            new Log
            {
                Id = 1,
                UserId = 1,
                Action = "Created",
                Timestamp = DateTime.UtcNow,
                Details = "User created",
                User = TestUser
            },
            new Log
            {
                Id = 2,
                UserId = 1,
                Action = "Edited",
                Timestamp = DateTime.UtcNow,
                Details = "User edited",
                User = TestUser
            }
        };
    }



    [Fact]
    public async Task LogActionAsync_ShouldCreateLog()
    {
        // Arrange
        var user = TestUser;
        var service = CreateService();

        // Act
        await service.LogActionAsync(user, "TestAction", "TestDetails");

        // Assert
        _dataContext.Verify(x => x.CreateAsync(It.Is<Log>(log =>
            log.UserId == user.Id &&
            log.Action == "TestAction" &&
            log.Details == "TestDetails" &&
            log.User == user &&
            log.Timestamp != default
        )), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLogsIncludingUsers()
    {
        // Arrange
        var logs = SetupLogs();
        _dataContext.Setup(c => c.GetAllIncludingAsync<Log>(It.IsAny<System.Linq.Expressions.Expression<Func<Log, object>>>()
))
            .ReturnsAsync(logs);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(logs, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnMatchingLogs()
    {
        // Arrange
        var logs = SetupLogs();
        _dataContext.Setup(c => c.GetAllIncludingAsync<Log>(It.IsAny<System.Linq.Expressions.Expression<Func<Log, object>>>()
))
            .ReturnsAsync(logs);

        var service = CreateService();

        // Act
        var result = await service.GetByUserIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(logs.Where(l => l.UserId == 1), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetByIdAsync_WhenLogExists_ReturnsLogWithUser()
    {
        // Arrange
        var logs = SetupLogs();
        _dataContext.Setup(c => c.GetAllIncludingAsync<Log>(It.IsAny<System.Linq.Expressions.Expression<Func<Log, object>>>()
))
            .ReturnsAsync(logs);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(logs.First(l => l.Id == 1));
    }

    [Fact]
    public async Task GetByIdAsync_WhenLogDoesNotExist_ReturnsNull()
    {
        // Arrange
        var logs = SetupLogs();
        _dataContext.Setup(c => c.GetAllIncludingAsync<Log>(It.IsAny<System.Linq.Expressions.Expression<Func<Log, object>>>()
))
            .ReturnsAsync(logs);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

}
