using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Services.Interfaces;

/// <summary>
/// Defines an interface for interacting with User Logs
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Creates a new log with the specified details
    /// </summary>
    /// <param name="user"> The user the action was performed on </param>
    /// <param name="action"> The action performed on the user </param>
    /// <param name="details"> Details of the action </param>
    Task LogActionAsync(User user, string action, string? details = null);

    /// <summary>
    /// Gets all Logs
    /// </summary>
    /// <returns> A List of all Logs </returns>
    Task<List<Log>> GetAllAsync();

    /// <summary>
    /// Get all logs associated with a specific User
    /// </summary>
    /// <param name="userId"> The Id of the User </param>
    /// <returns> A list of all Logs for the specified User </returns>
    Task<List<Log>> GetByUserIdAsync(long userId);

    /// <summary>
    /// Get a specific Log by its Id
    /// </summary>
    Task<Log?> GetByIdAsync(long id);

}
