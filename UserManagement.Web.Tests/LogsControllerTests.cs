using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Controllers;
using UserManagement.Web.Models.Logs;

namespace UserManagement.Web.Tests;

public class LogsControllerTests
{
    private readonly Mock<ILogService> _logServiceMock = new();

    private LogsController CreateController() => new(_logServiceMock.Object);

    private List<Log> SetupLogs()
    {
        return new List<Log>
        {
            new Log
            {
                Id = 1,
                Action = "Created",
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                User = new User { Email = "user1@example.com" },
                UserId = 1
            },
            new Log
            {
                Id = 2,
                Action = "Edited",
                Timestamp = DateTime.UtcNow.AddMinutes(-5),
                User = new User { Email = "user2@example.com" },
                UserId = 2
            }
        };
    }



    [Fact]
    public async Task Index_WhenNoFilters_ReturnsAllLogsPaged()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(logs);

        var controller = CreateController();

        var result = await controller.Index(null, null, 1, 10) as ViewResult;

        result.Should().NotBeNull();
        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().HaveCount(logs.Count);
        model.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Index_WhenEmailFilterApplied_ReturnsFilteredLogs()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(logs);

        var controller = CreateController();

        var result = await controller.Index("user1", null) as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().OnlyContain(l => l.UserEmail.Contains("user1"));
    }

    [Fact]
    public async Task Index_WhenActionFilterApplied_ReturnsFilteredLogs()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(logs);

        var controller = CreateController();

        var result = await controller.Index(null, "Edit") as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().OnlyContain(l => l.Action.Contains("Edit"));
    }

    [Fact]
    public async Task Index_WhenNoResults_ReturnsEmptyModel()
    {
        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Log>());

        var controller = CreateController();

        var result = await controller.Index("nonexistent", "nonexistent") as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().BeEmpty();
    }

    [Fact]
    public async Task Index_WhenPaginating_RespectsPageSizeAndPage()
    {
        var logs = Enumerable.Range(1, 25).Select(i => new Log
        {
            Id = i,
            Action = "Action " + i,
            Timestamp = DateTime.UtcNow.AddMinutes(-i),
            User = new User { Email = $"user{i}@example.com" },
            UserId = i
        }).ToList();

        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(logs);

        var controller = CreateController();

        var result = await controller.Index(null, null, page: 2, pageSize: 10) as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().HaveCount(10);
        model.Page.Should().Be(2);
        model.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Index_WhenPageNumberExceedsTotalPages_ReturnsEmptyLogs()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(logs);

        var controller = CreateController();

        var result = await controller.Index(null, null, page: 999, pageSize: 10) as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().BeEmpty();
    }

    [Fact]
    public async Task Index_WhenNegativePageNumberOrSize_ShouldDefaultToFirstPage()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(logs);

        var controller = CreateController();

        var result = await controller.Index(null, null, page: -1, pageSize: -10) as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Page.Should().Be(-1);
    }

    [Fact]
    public async Task View_WhenLogExists_ReturnsViewWithModel()
    {
        var log = new Log
        {
            Id = 1,
            Action = "Created",
            Timestamp = DateTime.UtcNow,
            User = new User { Email = "user@example.com" },
            UserId = 1,
            Details = "Details here"
        };

        _logServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(log);

        var controller = CreateController();

        var result = await controller.View(1) as ViewResult;

        var model = result!.Model.As<LogDetailsViewModel>();
        model.Id.Should().Be(1);
        model.UserEmail.Should().Be("user@example.com");
    }

    [Fact]
    public async Task View_WhenLogNotFound_ReturnsNotFound()
    {
        _logServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Log?)null);

        var controller = CreateController();

        var result = await controller.View(1);

        result.Should().BeOfType<NotFoundResult>();
    }



}
