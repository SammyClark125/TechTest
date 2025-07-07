using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

/// <summary>
/// Defines an interface for operations on User data
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get all users with a matching active/inactive status
    /// </summary>
    /// <param name="isActive"> true to return active users, false for inactive </param>
    /// <returns> A list of matching users </returns>
    IEnumerable<User> FilterByActive(bool isActive);

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns> A list of all users </returns>
    IEnumerable<User> GetAll();

    /// <summary>
    /// Get a single user by their ID
    /// </summary>
    /// <param name="id"> User ID </param>
    /// <returns> User if found, otherwise null </returns>
    User? GetById(long id);

    /// <summary>
    /// Add a new user to the system
    /// </summary>
    /// <param name="user"> The user to create </param>
    void CreateUser(User user);

    /// <summary>
    /// Update an existing user's details
    /// </summary>
    /// <param name="user"> User with updated info - matched by ID </param>
    void UpdateUser(User user);

    /// <summary>
    /// Delete a user if they exist
    /// </summary>
    /// <param name="user"> The user to delete </param>
    void DeleteUser(User user);

}
