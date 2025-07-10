using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Services.Interfaces;

/// <summary>
/// Defines an interface for asynchronously interacting with user logs.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Asynchronously creates a new log entry for the specified user and action.
    /// </summary>
    /// <param name="user">The user the action was performed on.</param>
    /// <param name="action">The action that was performed.</param>
    /// <param name="details">Optional details about the action.</param>
    /// <returns>A task that represents the asynchronous log creation operation.</returns>
    Task LogActionAsync(User user, string action, string? details = null);

    /// <summary>
    /// Asynchronously creates a new log entry for the specified user and action.
    /// </summary>
    /// <param name="userId">The id of the user the action was performed on</param>
    /// <param name="action">The action that was performed</param>
    /// <param name="details">Optional details about the action</param>
    /// <returns></returns>
    Task LogActionAsync(long userId, string action, string? details = null);

    /// <summary>
    /// Asynchronously retrieves all logs.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all logs.</returns>
    Task<List<Log>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves all logs associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose logs are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of logs for the specified user.</returns>
    Task<List<Log>> GetByUserIdAsync(long userId);

    /// <summary>
    /// Asynchronously retrieves a specific log by its ID.
    /// </summary>
    /// <param name="id">The ID of the log to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the log if found; otherwise, null.</returns>
    Task<Log?> GetByIdAsync(long id);

}
