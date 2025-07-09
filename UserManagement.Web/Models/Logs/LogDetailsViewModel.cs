using System;

namespace UserManagement.Web.Models.Logs;

public class LogDetailsViewModel
{
    public long Id { get; set; }
    public string UserEmail { get; set; } = "";
    public string Action { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }

}
