using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
        IsActive = true
    };

    private IQueryable<Log> SetupLogs()
    {
        var logs = new List<Log>
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
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Log>>();
        mockSet.As<IQueryable<Log>>().Setup(m => m.Provider).Returns(logs.Provider);
        mockSet.As<IQueryable<Log>>().Setup(m => m.Expression).Returns(logs.Expression);
        mockSet.As<IQueryable<Log>>().Setup(m => m.ElementType).Returns(logs.ElementType);
        mockSet.As<IQueryable<Log>>().Setup(m => m.GetEnumerator()).Returns(logs.GetEnumerator());

        _dataContext.Setup(c => c.GetAll<Log>()).Returns(mockSet.Object);

        return logs;
    }



    [Fact]
    public void LogAction_ShouldCreateLog()
    {
        // Arrange
        var user = TestUser;
        var service = CreateService();

        // Act
        service.LogAction(user, "TestAction", "TestDetails");

        // Assert
        _dataContext.Verify(x => x.Create(It.Is<Log>(log =>
            log.UserId == user.Id &&
            log.Action == "TestAction" &&
            log.Details == "TestDetails" &&
            log.User == user
        )), Times.Once);
    }

    [Fact]
    public void GetAll_ShouldReturnAllLogsIncludingUsers()
    {
        // Arrange
        var expectedLogs = SetupLogs();
        var service = CreateService();

        // Act
        var result = service.GetAll();

        // Assert
        result.Should().BeEquivalentTo(expectedLogs, options => options
        .IncludingNestedObjects());
    }

    [Fact]
    public void GetByUserId_ShouldReturnMatchingLogs()
    {
        // Arrange
        var logs = SetupLogs();
        var service = CreateService();

        // Act
        var result = service.GetByUserId(1);

        // Assert
        result.Should().BeEquivalentTo(logs.Where(l => l.UserId == 1));
    }

    [Fact]
    public void GetById_WhenLogExists_ReturnsLogWithUser()
    {
        // Arrange
        var logs = SetupLogs();
        var service = CreateService();

        // Act
        var result = service.GetById(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(logs.First(l => l.Id == 1), options => options
            .IncludingNestedObjects());
    }

    [Fact]
    public void GetById_WhenLogDoesNotExist_ReturnsNull()
    {
        // Arrange
        SetupLogs();
        var service = CreateService();

        // Act
        var result = service.GetById(999);

        // Assert
        result.Should().BeNull();
    }

}
