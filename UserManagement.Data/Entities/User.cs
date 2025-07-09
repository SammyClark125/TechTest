using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagement.Data.Entities;

namespace UserManagement.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }

    public ICollection<Log> Logs { get; set; } = new List<Log>();
}
