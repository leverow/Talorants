using Microsoft.AspNetCore.Identity;

namespace Talorants.Blog.Mvc.Entities;

public class AppUser : IdentityUser
{
    public string? Fullname { get; set; }
    public string? ImageUrl { get; set; }
    public ulong TelegramUserId { get; set; }
    public string[] Roles { get; set; }
    

    [Obsolete("Used only for entity binding")]
    public AppUser() { }

    public AppUser(string fullname, string username, string email)
    {
        Fullname = fullname;
        Email = email;
        UserName = username;
    }
}