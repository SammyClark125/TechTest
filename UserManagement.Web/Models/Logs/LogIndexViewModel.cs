namespace UserManagement.Web.Models.Logs;

public class LogIndexViewModel
{
    public IEnumerable<LogListItemViewModel> Logs { get; set; } = new List<LogListItemViewModel>();
    public string? EmailFilter { get; set; }
    public string? ActionFilter { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
