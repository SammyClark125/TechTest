using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;

public class LogService : ILogService
{
    private readonly IDataContext _dataAccess;
    public LogService(IDataContext dataAccess) => _dataAccess = dataAccess;



    public void LogAction(User user, string action, string? details = null)
    {
        Log newLog = new Log
        {
            UserId = user.Id,
            Action = action,
            Timestamp = DateTime.Now,
            Details = details,
            User = user
        };

        _dataAccess.Create<Log>(newLog);
    }

    public IEnumerable<Log> GetAll()
    {
        return _dataAccess.GetAll<Log>().Include(l => l.User);
    }

    public IEnumerable<Log> GetByUserId(long userId)
    {
        return _dataAccess.GetAll<Log>().Where(l => l.UserId == userId);
    }

    public Log? GetById(long id)
    {
        return _dataAccess.GetAll<Log>()
        .Include(l => l.User)
        .FirstOrDefault(l => l.Id == id);
    }

}
