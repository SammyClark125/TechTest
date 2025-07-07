using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;



    public IEnumerable<User> FilterByActive(bool isActive) => _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive);

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>().OrderBy(u => u.Id); // Deleting users caused weird listing behaviour without ordering

    public User? GetById(long id) => _dataAccess.GetByID<User>(id);

    public void CreateUser(User user)
    {
        _dataAccess.Create(user);
    }

    public void UpdateUser(User user)
    {
        _dataAccess.Update(user);
    }

    public void DeleteUser(User user)
    {
        var userCheck = _dataAccess.GetByID<User>(user.Id);
        if (userCheck != null)
        {
            _dataAccess.Delete(user);
        }
    }

}
