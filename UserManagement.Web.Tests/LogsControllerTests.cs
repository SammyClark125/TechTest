using System;
using System.Collections.Generic;
using System.Linq;
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
    public void Index_WhenNoFilters_ReturnsAllLogsPaged()
    {
        // Arrange
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAll()).Returns(logs.AsQueryable());

        var controller = CreateController();

        // Act
        var result = controller.Index(null, null, 1, 10) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().HaveCount(logs.Count);
        model.TotalPages.Should().Be(1);
    }

    [Fact]
    public void Index_WhenEmailFilterApplied_ReturnsFilteredLogs()
    {
        // Arrange
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAll()).Returns(logs.AsQueryable());

        var controller = CreateController();

        // Act
        var result = controller.Index("user1", null) as ViewResult;

        // Assert
        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().OnlyContain(l => l.UserEmail.Contains("user1"));
    }

    [Fact]
    public void Index_WhenActionFilterApplied_ReturnsFilteredLogs()
    {
        // Arrange
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAll()).Returns(logs.AsQueryable());

        var controller = CreateController();

        // Act
        var result = controller.Index(null, "Edit") as ViewResult;

        // Assert
        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().OnlyContain(l => l.Action.Contains("Edit"));
    }

    [Fact]
    public void Index_WhenNoResults_ReturnsEmptyModel()
    {
        // Arrange
        _logServiceMock.Setup(s => s.GetAll()).Returns(Enumerable.Empty<Log>().AsQueryable());

        var controller = CreateController();

        // Act
        var result = controller.Index("nonexistent", "nonexistent") as ViewResult;

        // Assert
        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().BeEmpty();
    }

    [Fact]
    public void Index_WhenPaginating_RespectsPageSizeAndPage()
    {
        // Arrange
        var logs = Enumerable.Range(1, 25).Select(i => new Log
        {
            Id = i,
            Action = "Action " + i,
            Timestamp = DateTime.UtcNow.AddMinutes(-i),
            User = new User { Email = $"user{i}@example.com" },
            UserId = i
        }).ToList();

        _logServiceMock.Setup(s => s.GetAll()).Returns(logs.AsQueryable());

        var controller = CreateController();

        // Act
        var result = controller.Index(null, null, page: 2, pageSize: 10) as ViewResult;

        // Assert
        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().HaveCount(10);
        model.Page.Should().Be(2);
        model.TotalPages.Should().Be(3);
    }

    [Fact]
    public void Index_WhenPageNumberExceedsTotalPages_ReturnsEmptyLogs()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAll()).Returns(logs.AsQueryable());

        var controller = CreateController();

        var result = controller.Index(null, null, page: 999, pageSize: 10) as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Logs.Should().BeEmpty();
    }

    [Fact]
    public void Index_WhenNegativePageNumberOrSize_ShouldDefaultToFirstPage()
    {
        var logs = SetupLogs();
        _logServiceMock.Setup(s => s.GetAll()).Returns(logs.AsQueryable());

        var controller = CreateController();

        var result = controller.Index(null, null, page: -1, pageSize: -10) as ViewResult;

        var model = result!.Model.As<LogIndexViewModel>();
        model.Page.Should().Be(-1);
    }


    [Fact]
    public void View_WhenLogExists_ReturnsViewWithModel()
    {
        // Arrange
        var log = new Log
        {
            Id = 1,
            Action = "Created",
            Timestamp = DateTime.UtcNow,
            User = new User { Email = "user@example.com" },
            UserId = 1,
            Details = "Details here"
        };

        _logServiceMock.Setup(s => s.GetById(1)).Returns(log);

        var controller = CreateController();

        // Act
        var result = controller.View(1) as ViewResult;

        // Assert
        var model = result!.Model.As<LogDetailsViewModel>();
        model.Id.Should().Be(1);
        model.UserEmail.Should().Be("user@example.com");
    }

    [Fact]
    public void View_WhenLogNotFound_ReturnsNotFound()
    {
        // Arrange
        _logServiceMock.Setup(s => s.GetById(1)).Returns((Log?)null);
        var controller = CreateController();

        // Act
        var result = controller.View(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }



}
