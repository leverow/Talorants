using System.Security.Claims;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Services;

public interface IUserManagementService
{
    ValueTask<bool> ExistsAsync(string? id);
    ValueTask<Result> CreateUserAsync(string fullName, string userName,string email, string password);
    ValueTask<Result<UserModel>> GetUserByIdAsync(string? id);
    ValueTask<Result> UpdateUserAsync(string? id, string fullName, string userName, string email, string password);
    ValueTask<Result> DeleteUserByIdAsync(string? id);
}