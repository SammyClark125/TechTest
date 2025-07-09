using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers();

        // Act
        var result = service.GetAll();

        // Assert
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void FilterByActive_WhenCalled_FiltersCorrectly()
    {
        // Arrange
        var activeUsers = SetupUsers(isActive: true);
        var service = CreateService();

        // Act
        var result = service.FilterByActive(true);

        // Assert
        result.Should().BeEquivalentTo(activeUsers);
    }

    [Fact]
    public void GetById_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1L, Email = "test@test.com" };
        _dataContext.Setup(x => x.GetByID<User>(1L)).Returns(user);
        var service = CreateService();

        // Act
        var result = service.GetById(1L);

        // Assert
        result.Should().BeSameAs(user);
    }

    [Fact]
    public void CreateUser_ShouldCallDataContextCreate()
    {
        // Arrange
        var user = new User();
        var service = CreateService();

        // Act
        service.CreateUser(user);

        // Assert
        _dataContext.Verify(x => x.Create(user), Times.Once);
    }

    [Fact]
    public void UpdateUser_ShouldCallDataContextUpdate()
    {
        // Arrange
        var user = new User();
        var service = CreateService();

        // Act
        service.UpdateUser(user);

        // Assert
        _dataContext.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public void DeleteUser_WhenUserExists_ShouldCallDelete()
    {
        // Arrange
        var user = new User { Id = 1 };
        _dataContext.Setup(x => x.GetByID<User>(user.Id)).Returns(user);
        var service = CreateService();

        // Act
        service.DeleteUser(user);

        // Assert
        _dataContext.Verify(x => x.Delete(user), Times.Once);
    }

    [Fact]
    public void DeleteUser_WhenUserNotFound_ShouldNotCallDelete()
    {
        // Arrange
        var user = new User { Id = 1 };
        _dataContext.Setup(x => x.GetByID<User>(user.Id)).Returns((User?)null);
        var service = CreateService();

        // Act
        service.DeleteUser(user);

        // Assert
        _dataContext.Verify(x => x.Delete(It.IsAny<User>()), Times.Never);
    }

}
