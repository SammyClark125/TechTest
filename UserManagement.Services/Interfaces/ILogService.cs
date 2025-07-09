using System.Collections.Generic;
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
    /// <param name="userId"> The Id of the user the action was performed on </param>
    /// <param name="action"> The action performed on the user </param>
    /// <param name="details"> Details of the action </param>
    void LogAction(User user, string action, string? details = null);

    /// <summary>
    /// Gets all Logs
    /// </summary>
    /// <returns> A List of all Logs </returns>
    IEnumerable<Log> GetAll();

    /// <summary>
    /// Get all logs associated with a specific User
    /// </summary>
    /// <param name="userId"> The Id of the User </param>
    /// <returns> A list of all Logs for the specified User </returns>
    IEnumerable<Log> GetByUserId(long userId);

    /// <summary>
    /// Get a specific Log by it's Id
    /// </summary>
    /// <returns> A single Log with the matching Id </returns>
    Log? GetById(long id);

}
