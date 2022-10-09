using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Talorants.Blog.Mvc.Entities;
using Talorants.Blog.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace Talorants.Blog.Mvc.Services;

public class UserManagementService : IUserManagementService
{
    private readonly ILogger<UserManagementService> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public UserManagementService(
        ILogger<UserManagementService> logger,
        UserManager<AppUser> userManager,
        IEmailSender emailSender
    )
    {
        _logger = logger;
        _userManager = userManager;
        _emailSender = emailSender;
    }
    public async ValueTask<Result> CreateUserAsync(string fullName, string userName, string email, string password)
    {
        var validationResult = Validate(fullName, userName, email);
        if(!validationResult.IsSuccess) return new("One or more fields are invalid.");

        var user = new AppUser(fullName,userName,email)
        {
            TwoFactorEnabled = true
        };
        try
        {
            var createUserResult = await _userManager.CreateAsync(user, password);
            if(createUserResult is null) return new("Couldn't create the user. Contact support.");

            if(!createUserResult.Succeeded)
            {
                var errors = string.Join('\n', createUserResult.Errors.Select(e => e.Code));
                return new(errors);
            }
            return new(true);
        }
        catch(DbUpdateException dbUpdateException)
        {
            _logger.LogInformation("Error occured:", dbUpdateException);
            return new("Couldn't create the user. Contact support.");
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't create the user. Contact support.", e);
        }
    }

    public async ValueTask<Result> DeleteUserByIdAsync(string? id)
    {
        if(string.IsNullOrWhiteSpace(id)) return new("User id is invalid.");
        try
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if(existingUser is null) return new("The user with given ID not found.");

            var deletedUser = await _userManager.DeleteAsync(existingUser);
            if(!deletedUser.Succeeded) return new("Removing the user failed. Contact support");

            return new(true);
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't delete the user. Contact support",e);
        }
    }


    public async ValueTask<Result<UserModel>> GetUserByIdAsync(string? id)
    {
        if(string.IsNullOrWhiteSpace(id)) return new("User id is invalid.");
        try
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if(existingUser is null) return new("The user with given ID not found");

            return new(true) { Data = ToModel(existingUser)};
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't retrieve the user. Contact support", e);
        }
    }

    public async ValueTask<Result> UpdateUserAsync(string? id, string fullName, string userName, string email, string password)
    {
        if(string.IsNullOrWhiteSpace(id)) return new("Id is invalid.");

        var validationResult = Validate(fullName, userName, email);
        if(!validationResult.IsSuccess) return new("One or more fields are invalid.");
        
        var existingUser = await _userManager.FindByIdAsync(id);
        existingUser.Fullname = fullName;
        existingUser.UserName = userName;
        existingUser.Email = email;

        try
        {
            var updateUserResult = await _userManager.UpdateAsync(existingUser);
            if(!updateUserResult.Succeeded) return new("Couldn't update the user. Contact support");

            return new(true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't update the user. Contact support", e);
        }
    }

    public async ValueTask<bool> ExistsAsync(string? id)
    => await _userManager.Users.AnyAsync(u => u.Id == id);

    private Result Validate(string fullname, string username, string? email)
    {
        if(string.IsNullOrWhiteSpace(fullname))
            return new("Invalid fullname.");
        
        if(string.IsNullOrWhiteSpace(username))
            return new("Invalid username.");
        
        if(!string.IsNullOrWhiteSpace(email) && !new EmailAddressAttribute().IsValid(email))
            return new("Invalid email.");

        return new(true);
    }

    private UserModel ToModel(AppUser entity)
    => new()
    {
        Id = entity.Id,
        FullName = entity.Fullname,
        Username = entity.UserName,
        Email = entity.Email,
        PasswordHash = entity.PasswordHash
    };
}