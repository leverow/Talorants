using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talorants.Blog.Mvc.Data;
using Talorants.Blog.Mvc.Entities;

namespace Talorants.Blog.Mvc.Extensions;

public static class UserManagerExtension
{
    public static async Task<AppUser> FindByTelegramIdAsync (this UserManager<AppUser> userManager, ulong telegramUserId)
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);
    }
}