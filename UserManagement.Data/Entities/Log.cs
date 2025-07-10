using System;
using UserManagement.Models;

namespace UserManagement.Data.Entities;

public class Log
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string Action { get; set; } = "";
    // public string? PerformedBy { get; set; } // If auth/logins are implemented
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }

    public User? User { get; set; }
}
