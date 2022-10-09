#pragma warning disable
using System.ComponentModel.DataAnnotations;

namespace Talorants.Blog.Mvc.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "FullName is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }

    [Required( ErrorMessage = "Username is required")]
    public string Username { get;  set; }
}