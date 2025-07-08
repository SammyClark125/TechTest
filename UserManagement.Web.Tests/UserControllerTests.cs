using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using UserManagement.Web.Models.Requests;
using System;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);

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
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void ViewUser_WhenUserExists_ReturnsViewWithUser()
    {
        var user = new User { Id = 1 };
        _userService.Setup(s => s.GetById(1)).Returns(user);

        var result = CreateController().ViewUser(1) as ViewResult;

        result?.Model.Should().BeSameAs(user);
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


}
