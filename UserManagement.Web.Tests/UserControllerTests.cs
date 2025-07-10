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
using System.Threading.Tasks;

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
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(users.ToList());

        return users;
    }



    [Fact]
    public async Task List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
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

        var result = await controller.List();

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(expectedViewModels);
    }

    [Fact]
    public async Task ViewUser_WhenUserExists_ReturnsViewWithUserLogsViewModel()
    {
        var user = new User { Id = 1 };
        var logs = Enumerable.Empty<Log>();
        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);
        _logService.Setup(s => s.GetByUserIdAsync(1)).ReturnsAsync(logs.ToList());

        var result = await CreateController().ViewUser(1) as ViewResult;

        result.Should().NotBeNull();
        var model = result!.Model.Should().BeOfType<UserLogsViewModel>().Subject;
        model.User.Should().BeSameAs(user);
        model.Logs.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public async Task ViewUser_WhenUserNotFound_ReturnsNotFound()
    {
        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((User?)null);

        var result = await CreateController().ViewUser(1);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddUser_Post_WhenValid_ShouldCreateAndRedirect()
    {
        var request = new AddUserRequest
        {
            Forename = "Alice",
            Surname = "Smith",
            Email = "alice@example.com",
            DateOfBirth = new DateOnly(),
            IsActive = true
        };

        var controller = CreateController();

        var result = await controller.AddUser(request) as RedirectToActionResult;

        result!.ActionName.Should().Be("List");
        _userService.Verify(s => s.CreateUserAsync(It.Is<User>(u =>
            u.Forename == request.Forename &&
            u.Surname == request.Surname &&
            u.Email == request.Email &&
            u.DateOfBirth == request.DateOfBirth &&
            u.IsActive == request.IsActive
        )), Times.Once);
    }

    [Fact]
    public async Task AddUser_Post_WithInvalidModelState_ShouldReturnView()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Error", "Invalid");

        var request = new AddUserRequest();

        var result = await controller.AddUser(request) as ViewResult;

        result.Should().NotBeNull();
        result!.Model.Should().BeSameAs(request);
    }

    [Fact]
    public async Task EditUser_Post_WhenValid_ShouldUpdateAndRedirect()
    {
        var existing = new User { Id = 1, Forename = "John" };
        _userService.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        var updated = new User { Id = 1, Forename = "Jane" };

        var result = await CreateController().EditUser(updated) as RedirectToActionResult;

        result!.ActionName.Should().Be("List");
        existing.Forename.Should().Be("Jane");
        _userService.Verify(s => s.UpdateUserAsync(existing), Times.Once);
    }

    [Fact]
    public async Task EditUser_Post_WhenUserNotFound_ShouldReturnNotFound()
    {
        var updated = new User { Id = 99, Forename = "Ghost" };
        _userService.Setup(x => x.GetByIdAsync(updated.Id)).ReturnsAsync((User?)null);

        var result = await CreateController().EditUser(updated);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldDeleteAndRedirect()
    {
        var user = new User { Id = 1 };
        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await CreateController().DeleteUser(1) as RedirectToActionResult;

        result!.ActionName.Should().Be("List");
        _userService.Verify(s => s.DeleteUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ShouldReturnNotFound()
    {
        _userService.Setup(s => s.GetByIdAsync(42)).ReturnsAsync((User?)null);

        var result = await CreateController().DeleteUser(42);

        result.Should().BeOfType<NotFoundResult>();
        _userService.Verify(s => s.DeleteUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddUser_Post_WhenValid_ShouldLogCreation()
    {
        var request = new AddUserRequest
        {
            Forename = "Log",
            Surname = "User",
            Email = "loguser@example.com",
            DateOfBirth = new DateOnly(2000, 1, 1),
            IsActive = true
        };

        var controller = CreateController();

        await controller.AddUser(request);

        _logService.Verify(log =>
            log.LogActionAsync(It.Is<User>(u => u.Email == request.Email), "Created", $"Created user {request.Email}"),
            Times.Once);
    }

    [Fact]
    public async Task AddUser_Post_WithInvalidModelState_ShouldNotLog()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Email", "Required");

        var request = new AddUserRequest { Email = "bad@example.com" };

        await controller.AddUser(request);

        _logService.Verify(log => log.LogActionAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EditUser_Post_WhenValid_ShouldLogEdit()
    {
        var existing = new User { Id = 1, Email = "existing@example.com" };
        _userService.Setup(s => s.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        var updated = new User
        {
            Id = 1,
            Forename = "Edited",
            Surname = "User",
            Email = "existing@example.com",
            DateOfBirth = new DateOnly(1995, 5, 5),
            IsActive = false
        };

        await CreateController().EditUser(updated);

        _logService.Verify(log =>
            log.LogActionAsync(existing, "Edited", $"Edited user {existing.Email}"),
            Times.Once);
    }

    [Fact]
    public async Task EditUser_Post_WithInvalidModelState_ShouldNotLog()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Email", "Required");

        var updated = new User { Id = 1, Email = "invalid@example.com" };

        await controller.EditUser(updated);

        _logService.Verify(log => log.LogActionAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EditUser_Post_WhenUserNotFound_ShouldNotLog()
    {
        var updated = new User { Id = 999, Email = "ghost@example.com" };
        _userService.Setup(s => s.GetByIdAsync(updated.Id)).ReturnsAsync((User?)null);

        await CreateController().EditUser(updated);

        _logService.Verify(log => log.LogActionAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldLogDeletion()
    {
        var user = new User { Id = 1, Email = "delete@example.com" };
        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);

        await CreateController().DeleteUser(1);

        _logService.Verify(log =>
            log.LogActionAsync(user.Id, "Deleted", $"Deleted user {user.Email}"),
            Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ShouldNotLog()
    {
        _userService.Setup(s => s.GetByIdAsync(42)).ReturnsAsync((User?)null);

        await CreateController().DeleteUser(42);

        _logService.Verify(log => log.LogActionAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

}
