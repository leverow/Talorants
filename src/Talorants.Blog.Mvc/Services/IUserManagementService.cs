using System.Security.Claims;
using Talorants.Blog.Mvc.Entities;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Services;

public interface IUserManagementService
{
    ValueTask<bool> ExistsAsync(string? id);
    ValueTask<bool> IsTelegramUserIdExistsAsync(ulong telegramUserId);
    ValueTask<Result> CreateUserAsync(string fullName, string userName,string email, string password, IFormFile userImage);
    ValueTask<Result> CreateUserAsync(ulong id, string? firstName, string? lastName, string? userName, string? userImageUrl);
    ValueTask<Result<UserModel>> GetUserByIdAsync(string? id);
    ValueTask<Result<AppUser>> GetUserByTelegramIdAsync(ulong id);
    ValueTask<Result<UserModel>> GetUserByUserNameAsync(string? userName);
    ValueTask<Result<string>> GetUserPhotoByUserNameAsync(string? userName);
    ValueTask<Result> UpdateUserAsync(string? id, string fullName, string userName, string email, string password, IFormFile userImage);
    ValueTask<Result> DeleteUserByIdAsync(string? id);

    bool IsAuthorizedInTelegram(ulong id, string? first_name, string? last_name, string? username, string? photo_url, ulong auth_date, string? checkHash, string key);
}