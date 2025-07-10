using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

/// <summary>
/// Defines an interface for asynchronously performing operations on user data.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Asynchronously retrieves all users matching the specified active or inactive status.
    /// </summary>
    /// <param name="isActive">True to retrieve active users; false to retrieve inactive users.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of matching users.</returns>
    Task<List<User>> FilterByActiveAsync(bool isActive);

    /// <summary>
    /// Asynchronously retrieves all users.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all users.</returns>
    Task<List<User>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<User?> GetByIdAsync(long id);

    /// <summary>
    /// Asynchronously creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    Task CreateUserAsync(User user);

    /// <summary>
    /// Asynchronously updates an existing user's information.
    /// </summary>
    /// <param name="user">The user with updated details. The user is identified by ID.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateUserAsync(User user);

    /// <summary>
    /// Asynchronously deletes the specified user if they exist.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteUserAsync(User user);

}
