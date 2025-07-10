using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;

public class LogService : ILogService
{
    private readonly IDataContext _dataAccess;
    public LogService(IDataContext dataAccess) => _dataAccess = dataAccess;



    public async Task LogActionAsync(User user, string action, string? details = null)
    {
        var newLog = new Log
        {
            UserId = user.Id,
            Action = action,
            Timestamp = DateTime.Now,
            Details = details,
            User = user
        };

        await _dataAccess.CreateAsync(newLog);
    }

    public async Task<List<Log>> GetAllAsync()
    {
        return await _dataAccess.GetAllIncludingAsync<Log>(l => l.User);
    }

    public async Task<List<Log>> GetByUserIdAsync(long userId)
    {
        var logs = await _dataAccess.GetAllIncludingAsync<Log>(l => l.User);
        return logs.Where(l => l.UserId == userId).ToList();
    }

    public async Task<Log?> GetByIdAsync(long id)
    {
        return (await _dataAccess
            .GetAllIncludingAsync<Log>(l => l.User))
            .FirstOrDefault(l => l.Id == id);
    }

}
