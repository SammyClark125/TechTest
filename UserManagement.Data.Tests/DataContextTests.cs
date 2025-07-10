using System;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{

    private DataContext CreateContext() => new();

    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com",
            DateOfBirth = new DateOnly(1990, 1, 1),
            IsActive = true
        };
        await context.CreateAsync(entity);

        // Act
        var result = await context.GetAllIncludingAsync<User>();

        // Assert
        result.Should().Contain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange
        var context = CreateContext();
        var original = new User
        {
            Forename = "Temp",
            Surname = "User",
            Email = "tempuser@example.com",
            DateOfBirth = new DateOnly(1991, 2, 2),
            IsActive = true
        };
        await context.CreateAsync(original);

        // Sanity check
        (await context.GetAllIncludingAsync<User>()).Should().Contain(u => u.Email == original.Email);

        // Act
        await context.DeleteAsync(original);
        var result = await context.GetAllIncludingAsync<User>();

        // Assert
        result.Should().NotContain(s => s.Email == original.Email);
    }

    [Fact]
    public async Task GetByID_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var context = CreateContext();
        var user = new User
        {
            Forename = "Alice",
            Surname = "Jones",
            Email = "alice@test.com",
            DateOfBirth = new DateOnly(1985, 5, 5),
            IsActive = true
        };
        await context.CreateAsync(user);

        // Act
        var result = await context.GetByIDAsync<User>(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user, options => options.Excluding(u => u.Logs));
    }

    [Fact]
    public async Task GetByID_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var result = await context.GetByIDAsync<User>(999L); // unlikely to exist

        // Assert
        result.Should().BeNull();
    }

}
