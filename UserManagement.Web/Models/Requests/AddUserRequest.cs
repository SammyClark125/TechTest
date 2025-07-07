using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Requests;

public class AddUserRequest
{
    [Required]
    public string Forename { get; set; } = "";

    [Required]
    public string Surname { get; set; }  = "";

    [Required]
    public string Email { get; set; }  = "";

    [Required]
    public DateOnly DateOfBirth { get; set; } = new DateOnly();

    public bool IsActive { get; set; }
}
