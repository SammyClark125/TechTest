using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Requests;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    public UsersController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    [HttpGet]
    public ViewResult List()
    {
        var items = _userService.GetAll().Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            DateOfBirth = p.DateOfBirth,
            Email = p.Email,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View("List", model);
    }

    [HttpGet("Active")]
    public ViewResult ListActive()
    {
        var items = _userService.FilterByActive(true).Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            DateOfBirth = p.DateOfBirth,
            Email = p.Email,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View("List", model);
    }

    [HttpGet("NonActive")]
    public ViewResult ListNonActive()
    {
        var items = _userService.FilterByActive(false).Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            DateOfBirth = p.DateOfBirth,
            Email = p.Email,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View("List", model);
    }



    // Add user
    [HttpGet("AddUser")]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost("AddUser")]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(AddUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var user = new User
        {
            Forename = request.Forename,
            Surname = request.Surname,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            IsActive = request.IsActive
        };

        _userService.CreateUser(user);
        _logService.LogAction(user, "Created", $"Created user {user.Email}");

        return RedirectToAction("List");
    }



    // View User
    [HttpGet("ViewUser")]
    public IActionResult ViewUser(long id)
    {
        var user = _userService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserLogsViewModel
        {
            User = user,
            Logs = _logService.GetByUserId(user.Id)
        };

        return View(model);
    }



    // Edit User
    [HttpGet("EditUser")]
    public IActionResult EditUser(long id)
    {
        var user = _userService.GetById(id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost("EditUser")]
    [ValidateAntiForgeryToken]
    public IActionResult EditUser(User updatedUser)
    {
        if (!ModelState.IsValid)
        {
            return View(updatedUser);
        }

        var existingUser = _userService.GetById(updatedUser.Id);

        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Forename = updatedUser.Forename;
        existingUser.Surname = updatedUser.Surname;
        existingUser.DateOfBirth = updatedUser.DateOfBirth;
        existingUser.Email = updatedUser.Email;
        existingUser.IsActive = updatedUser.IsActive;

        _userService.UpdateUser(existingUser);
        _logService.LogAction(existingUser, "Edited", $"Edited user {existingUser.Email}");
        return RedirectToAction("List");
    }



    // Delete User
    [HttpPost("DeleteUser")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteUser(long id)
    {
        var user = _userService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        _userService.DeleteUser(user);
        _logService.LogAction(user, "Deleted", $"Deleted user {user.Email}");
        return RedirectToAction("List");
    }



}
