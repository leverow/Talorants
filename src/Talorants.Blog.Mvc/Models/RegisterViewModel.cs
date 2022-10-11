#pragma warning disable
using System.ComponentModel.DataAnnotations;
using Talorants.Blog.Mvc.CustomAttributes;

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
    [Compare("Password", ErrorMessage = "Password must be the same")]
    public string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }

    [Required( ErrorMessage = "Username is required")]
    public string Username { get;  set; }

    [Required(ErrorMessage = "UserImage is required")]
    [DataType(DataType.Upload)]
    [MaxFileSize(5* 1024 * 1024)]
    [AllowedExtensions(new string[] { ".jpg", ".png" })]
    public IFormFile UserImage { get; set; }
    
    public string[] Roles { get; set; }   
}