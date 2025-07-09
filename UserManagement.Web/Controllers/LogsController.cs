using System;
using System.Linq;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Logs;

namespace UserManagement.Web.Controllers;

[Route("logs")]
public class LogsController : Controller
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }



    [HttpGet("Index")]
    public IActionResult Index(string? email, string? actionFilter, int page = 1, int pageSize = 10)
    {
        // Get all logs
        var logs = _logService.GetAll();

        // Filter by Email
        if (!string.IsNullOrWhiteSpace(email))
        {
            email = email.Trim();
            logs = logs.Where(l => l.User.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by Action
        if (!string.IsNullOrWhiteSpace(actionFilter))
        {
            logs = logs.Where(l => l.Action.Contains(actionFilter, StringComparison.OrdinalIgnoreCase));
        }

        // Splitting into pages
        var total = logs.Count();
        var paged = logs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Create model
        if (logs.Any())
        {
            var model = new LogIndexViewModel
            {
                Logs = paged.Select(l => new LogListItemViewModel
                {
                    Id = l.Id,
                    Action = l.Action,
                    Timestamp = l.Timestamp,
                    UserEmail = l.User.Email
                }),
                EmailFilter = email,
                ActionFilter = actionFilter,
                Page = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };

            return View(model);
        }

        else return View(new LogIndexViewModel());
    }

    [HttpGet("View")]
    public IActionResult View(long id)
    {
        var log = _logService.GetById(id);

        if (log == null)
        {
            return NotFound();
        }

        var model = new LogDetailsViewModel
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Action = log.Action,
            Details = log.Details,
            UserEmail = log.User.Email
        };

        return View(model);
    }

}
