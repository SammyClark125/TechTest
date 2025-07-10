using System.Linq;
using System.Threading.Tasks;
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
    public async Task<ViewResult> List()
    {
        var items = (await _userService.GetAllAsync()).Select(p => new UserListItemViewModel
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
    public async Task<ViewResult> ListActive()
    {
        var items = (await _userService.FilterByActiveAsync(true)).Select(p => new UserListItemViewModel
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
    public async Task<ViewResult> ListNonActive()
    {
        var items = (await _userService.FilterByActiveAsync(false)).Select(p => new UserListItemViewModel
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

    [HttpGet("AddUser")]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost("AddUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(AddUserRequest request)
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

        await _userService.CreateUserAsync(user);
        await _logService.LogActionAsync(user, "Created", $"Created user {user.Email}");

        return RedirectToAction("List");
    }

    [HttpGet("ViewUser")]
    public async Task<IActionResult> ViewUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var logs = await _logService.GetByUserIdAsync(user.Id);

        var model = new UserLogsViewModel
        {
            User = user,
            Logs = logs
        };

        return View(model);
    }

    [HttpGet("EditUser")]
    public async Task<IActionResult> EditUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost("EditUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(User updatedUser)
    {
        if (!ModelState.IsValid)
        {
            return View(updatedUser);
        }

        var existingUser = await _userService.GetByIdAsync(updatedUser.Id);

        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Forename = updatedUser.Forename;
        existingUser.Surname = updatedUser.Surname;
        existingUser.DateOfBirth = updatedUser.DateOfBirth;
        existingUser.Email = updatedUser.Email;
        existingUser.IsActive = updatedUser.IsActive;

        await _userService.UpdateUserAsync(existingUser);
        await _logService.LogActionAsync(existingUser, "Edited", $"Edited user {existingUser.Email}");

        return RedirectToAction("List");
    }

    [HttpPost("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _userService.DeleteUserAsync(user);
        await _logService.LogActionAsync(user, "Deleted", $"Deleted user {user.Email}");

        return RedirectToAction("List");
    }

}
