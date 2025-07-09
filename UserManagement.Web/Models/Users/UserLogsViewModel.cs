using System.Linq;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Web.Models.Users;

public class UserLogsViewModel
{
    public User User { get; set; } = default!;
    public IEnumerable<Log> Logs { get; set; } = Enumerable.Empty<Log>();
}
