using System.Security.Claims;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Services;

public interface IUserManagementService
{
    ValueTask<bool> ExistsAsync(string? id);
    ValueTask<Result> CreateUserAsync(string fullName, string userName,string email, string password, IFormFile userImage);
    ValueTask<Result<UserModel>> GetUserByIdAsync(string? id);
    ValueTask<Result<UserModel>> GetUserByUserNameAsync(string? userName);
    ValueTask<Result<string>> GetUserPhotoByUserNameAsync(string? userName);
    ValueTask<Result> UpdateUserAsync(string? id, string fullName, string userName, string email, string password, IFormFile userImage);
    ValueTask<Result> DeleteUserByIdAsync(string? id);
}