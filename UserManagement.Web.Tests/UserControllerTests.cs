using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using UserManagement.Web.Models.Requests;
using System;
using UserManagement.Services.Interfaces;
using System.Linq;
using UserManagement.Data.Entities;

namespace UserManagement.Web.Tests;

public class UserControllerTests
{

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<ILogService> _logService = new();
    private UsersController CreateController() => new(_userService.Object, _logService.Object);

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
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
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        return users;
    }



    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange
        var controller = CreateController();
        var users = SetupUsers();

        var expectedViewModels = users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            Forename = u.Forename,
            Surname = u.Surname,
            DateOfBirth = u.DateOfBirth,
            Email = u.Email,
            IsActive = u.IsActive
        });

        // Act
        var result = controller.List();

        // Assert
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(expectedViewModels);
    }

    [Fact]
    public void ViewUser_WhenUserExists_ReturnsViewWithUserLogsViewModel()
    {
        // Arrange
        var user = new User { Id = 1 };
        var logs = Enumerable.Empty<Log>();
        _userService.Setup(s => s.GetById(1)).Returns(user);
        _logService.Setup(s => s.GetByUserId(1)).Returns(logs);

        // Act
        var result = CreateController().ViewUser(1) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model.Should().BeOfType<UserLogsViewModel>().Subject;
        model.User.Should().BeSameAs(user);
        model.Logs.Should().BeSameAs(logs);
    }


    [Fact]
    public void ViewUser_WhenUserNotFound_ReturnsNotFound()
    {
        _userService.Setup(s => s.GetById(1)).Returns((User?)null);

        var result = CreateController().ViewUser(1);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void AddUser_Post_WhenValid_ShouldCreateAndRedirect()
    {
        // Arrange
        var request = new AddUserRequest
        {
            Forename = "Alice",
            Surname = "Smith",
            Email = "alice@example.com",
            DateOfBirth = new DateOnly(),
            IsActive = true
        };

        var controller = CreateController();

        // Act
        var result = controller.AddUser(request) as RedirectToActionResult;

        // Assert
        result!.ActionName.Should().Be("List");
        _userService.Verify(s => s.CreateUser(It.Is<User>(u =>
            u.Forename == request.Forename &&
            u.Surname == request.Surname &&
            u.Email == request.Email &&
            u.DateOfBirth == request.DateOfBirth &&
            u.IsActive == request.IsActive
        )), Times.Once);
    }


    [Fact]
    public void AddUser_Post_WithInvalidModelState_ShouldReturnView()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Error", "Invalid");

        var request = new AddUserRequest(); // can be empty or invalid

        var result = controller.AddUser(request) as ViewResult;

        result.Should().NotBeNull();
        result!.Model.Should().BeSameAs(request);
    }

    [Fact]
    public void EditUser_Post_WhenValid_ShouldUpdateAndRedirect()
    {
        var existing = new User { Id = 1, Forename = "John" };
        _userService.Setup(x => x.GetById(existing.Id)).Returns(existing);

        var updated = new User { Id = 1, Forename = "Jane" };

        var result = CreateController().EditUser(updated) as RedirectToActionResult;

        result!.ActionName.Should().Be("List");
        existing.Forename.Should().Be("Jane");
        _userService.Verify(s => s.UpdateUser(existing), Times.Once);
    }

    [Fact]
    public void EditUser_Post_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var updated = new User { Id = 99, Forename = "Ghost" };
        _userService.Setup(x => x.GetById(updated.Id)).Returns((User?)null);

        // Act
        var result = CreateController().EditUser(updated);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }


    [Fact]
    public void DeleteUser_WhenUserExists_ShouldDeleteAndRedirect()
    {
        var user = new User { Id = 1 };
        _userService.Setup(s => s.GetById(1)).Returns(user);

        var result = CreateController().DeleteUser(1) as RedirectToActionResult;

        result!.ActionName.Should().Be("List");
        _userService.Verify(s => s.DeleteUser(user), Times.Once);
    }

    [Fact]
    public void DeleteUser_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _userService.Setup(s => s.GetById(42)).Returns((User?)null);

        // Act
        var result = CreateController().DeleteUser(42);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _userService.Verify(s => s.DeleteUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void AddUser_Post_WhenValid_ShouldLogCreation()
    {
        // Arrange
        var request = new AddUserRequest
        {
            Forename = "Log",
            Surname = "User",
            Email = "loguser@example.com",
            DateOfBirth = new DateOnly(2000, 1, 1),
            IsActive = true
        };

        var controller = CreateController();

        // Act
        controller.AddUser(request);

        // Assert
        _logService.Verify(log =>
            log.LogAction(It.Is<User>(u => u.Email == request.Email), "Created", $"Created user {request.Email}"),
            Times.Once);
    }

    [Fact]
    public void AddUser_Post_WithInvalidModelState_ShouldNotLog()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Email", "Required");

        var request = new AddUserRequest { Email = "bad@example.com" };

        controller.AddUser(request);

        _logService.Verify(log => log.LogAction(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void EditUser_Post_WhenValid_ShouldLogEdit()
    {
        // Arrange
        var existing = new User { Id = 1, Email = "existing@example.com" };
        _userService.Setup(s => s.GetById(existing.Id)).Returns(existing);

        var updated = new User
        {
            Id = 1,
            Forename = "Edited",
            Surname = "User",
            Email = "existing@example.com",
            DateOfBirth = new DateOnly(1995, 5, 5),
            IsActive = false
        };

        // Act
        CreateController().EditUser(updated);

        // Assert
        _logService.Verify(log =>
            log.LogAction(existing, "Edited", $"Edited user {existing.Email}"),
            Times.Once);
    }

    [Fact]
    public void EditUser_Post_WithInvalidModelState_ShouldNotLog()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Email", "Required");

        var updated = new User { Id = 1, Email = "invalid@example.com" };

        controller.EditUser(updated);

        _logService.Verify(log => log.LogAction(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void EditUser_Post_WhenUserNotFound_ShouldNotLog()
    {
        var updated = new User { Id = 999, Email = "ghost@example.com" };
        _userService.Setup(s => s.GetById(updated.Id)).Returns((User?)null);

        CreateController().EditUser(updated);

        _logService.Verify(log => log.LogAction(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void DeleteUser_WhenUserExists_ShouldLogDeletion()
    {
        var user = new User { Id = 1, Email = "delete@example.com" };
        _userService.Setup(s => s.GetById(1)).Returns(user);

        CreateController().DeleteUser(1);

        _logService.Verify(log =>
            log.LogAction(user, "Deleted", $"Deleted user {user.Email}"),
            Times.Once);
    }

    [Fact]
    public void DeleteUser_WhenUserNotFound_ShouldNotLog()
    {
        _userService.Setup(s => s.GetById(42)).Returns((User?)null);

        CreateController().DeleteUser(42);

        _logService.Verify(log => log.LogAction(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

}
