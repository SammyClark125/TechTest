using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);

    private List<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                DateOfBirth = new DateOnly(1990, 1, 1)
            }
        };

        _dataContext
            .Setup(s => s.GetAllIncludingAsync<User>())
            .ReturnsAsync(users);

        return users;
    }

    [Fact]
    public async Task GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(users.OrderBy(u => u.Id));
    }

    [Fact]
    public async Task FilterByActive_WhenCalled_FiltersCorrectly()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, IsActive = true, Email = "active1@example.com", DateOfBirth = new DateOnly(1990,1,1) },
            new User { Id = 2, IsActive = false, Email = "inactive@example.com", DateOfBirth = new DateOnly(1990,1,1) },
            new User { Id = 3, IsActive = true, Email = "active2@example.com", DateOfBirth = new DateOnly(1990,1,1) }
        };

        _dataContext.Setup(s => s.GetAllIncludingAsync<User>())
            .ReturnsAsync(users);

        var service = CreateService();

        // Act
        var result = await service.FilterByActiveAsync(true);

        // Assert
        result.Should().OnlyContain(u => u.IsActive);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1L, Email = "test@test.com", DateOfBirth = new DateOnly(1990, 1, 1) };
        _dataContext.Setup(x => x.GetByIDAsync<User>(1L)).ReturnsAsync(user);
        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(1L);

        // Assert
        result.Should().BeSameAs(user);
    }

    [Fact]
    public async Task CreateUser_ShouldCallDataContextCreateAsync()
    {
        // Arrange
        var user = new User();
        var service = CreateService();

        // Act
        await service.CreateUserAsync(user);

        // Assert
        _dataContext.Verify(x => x.CreateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldCallDataContextUpdateAsync()
    {
        // Arrange
        var user = new User();
        var service = CreateService();

        // Act
        await service.UpdateUserAsync(user);

        // Assert
        _dataContext.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldCallDeleteAsync()
    {
        // Arrange
        var user = new User { Id = 1 };
        _dataContext.Setup(x => x.GetByIDAsync<User>(user.Id)).ReturnsAsync(user);
        var service = CreateService();

        // Act
        await service.DeleteUserAsync(user);

        // Assert
        _dataContext.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ShouldNotCallDelete()
    {
        // Arrange
        var user = new User { Id = 1 };
        _dataContext.Setup(x => x.GetByIDAsync<User>(user.Id)).ReturnsAsync((User?)null);
        var service = CreateService();

        // Act
        await service.DeleteUserAsync(user);

        // Assert
        _dataContext.Verify(x => x.DeleteAsync(It.IsAny<User>()), Times.Never);
    }

}
