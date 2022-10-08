using Microsoft.AspNetCore.Identity;

namespace Talorants.Blog.Mvc.Entities;

public class AppUser : IdentityUser
{
    public string? ImageUrl { get; set; }
}