using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;



    public async Task<List<User>> FilterByActiveAsync(bool isActive)
    {
        var users = await _dataAccess.GetAllIncludingAsync<User>();
        return users.Where(u => u.IsActive == isActive).ToList();
    }

    public async Task<List<User>> GetAllAsync()
    {
        var users = await _dataAccess.GetAllIncludingAsync<User>();
        return users.OrderBy(u => u.Id).ToList();
    }

    public async Task<User?> GetByIdAsync(long id) => await _dataAccess.GetByIDAsync<User>(id);

    public async Task CreateUserAsync(User user) => await _dataAccess.CreateAsync(user);

    public async Task UpdateUserAsync(User user) => await _dataAccess.UpdateAsync(user);

    public async Task DeleteUserAsync(User user)
    {
        var userCheck = await _dataAccess.GetByIDAsync<User>(user.Id);
        if (userCheck != null)
        {
            await _dataAccess.DeleteAsync(userCheck);
        }
    }

}
