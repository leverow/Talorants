using System.Net;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Talorants.Blog.Mvc.Entities;
using Talorants.Blog.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Talorants.Blog.Mvc.Extensions;
using System.Security.Cryptography;
using System.Text;

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
    public async ValueTask<Result> CreateUserAsync(string fullName, string userName, string email, string password,IFormFile userImage)
    {
        var validationResult = Validate(fullName, userName, email, userImage);
        if(!validationResult.IsSuccess) return new("One or more fields are invalid.");

        var savedUserImageResult = await SaveUserImageAsync(userImage);
        var user = new AppUser(fullName,userName,email)
        {
            TwoFactorEnabled = true,
            ImageUrl = savedUserImageResult.Data
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

    public async ValueTask<Result<UserModel>> GetUserByUserNameAsync(string? userName)
    {
        if(string.IsNullOrWhiteSpace(userName)) return new("Username is invalid.");
        try
        {
            var existingUser = await _userManager.FindByNameAsync(userName);
            if(existingUser is null) return new("The user with given username not found");

            return new(true) { Data = ToModel(existingUser)};
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't retrieve the user. Contact support", e);
        }
    }

    public async ValueTask<Result<string>> GetUserPhotoByUserNameAsync(string? userName)
    {
        if(string.IsNullOrWhiteSpace(userName)) return new("Username is invalid.");
        try
        {
            var existingUser = await _userManager.FindByNameAsync(userName);
            if(existingUser is null) return new("The user with given username not found");

            return new(true) { Data = existingUser.ImageUrl ?? "avatar.png"};
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't retrieve profile image. Contact support", e);
        }
    }

    public async ValueTask<Result> UpdateUserAsync(string? id, string fullName, string userName, string email, string password, IFormFile userImage)
    {
        if(string.IsNullOrWhiteSpace(id)) return new("Id is invalid.");

        var validationResult = Validate(fullName, userName, email, userImage);
        if(!validationResult.IsSuccess) return new("One or more fields are invalid.");
        
        var savedUserImageResult = await SaveUserImageAsync(userImage);

        var existingUser = await _userManager.FindByIdAsync(id);
        existingUser.Fullname = fullName;
        existingUser.UserName = userName;
        existingUser.Email = email;
        existingUser.ImageUrl = savedUserImageResult.Data;

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

    private Result Validate(string fullname, string username, string? email, IFormFile image)
    {
        if(string.IsNullOrWhiteSpace(fullname))
            return new("Invalid fullname.");
        
        if(string.IsNullOrWhiteSpace(username))
            return new("Invalid username.");
        
        if(!string.IsNullOrWhiteSpace(email) && !new EmailAddressAttribute().IsValid(email))
            return new("Invalid email.");
        
        var supportedTypes = new[] { "jpg", "png"};  
        var fileExt = System.IO.Path.GetExtension(image.FileName).Substring(1);  
        if (!supportedTypes.Contains(fileExt))  
            return new("Image extension is invalid - Only Upload Jpg/Png File");
        
        return new(true);
    }

    private UserModel ToModel(AppUser entity)
    => new()
    {
        Id = entity.Id,
        FullName = entity.Fullname,
        Username = entity.UserName,
        Email = entity.Email,
        PasswordHash = entity.PasswordHash,
        UserImageUrl = entity.ImageUrl
    };

    private async Task<Result<string>> SaveUserImageAsync(IFormFile? userImageFile)
    {
        if(userImageFile is null) return new(false) { Data = "avatar.png" };

        var imagePath = Guid.NewGuid().ToString("N") + Path.GetExtension(userImageFile.FileName);

        var ms = new MemoryStream();
        await userImageFile.CopyToAsync(ms);
        System.IO.File.WriteAllBytes(Path.Combine(new string[5]{ "wwwroot", "Media", "User", "Images", imagePath }), ms.ToArray());

        return new(true) { Data = imagePath };
    }

    private async Task<Result<string>> SaveUserImageAsync(string? imageUrl)
    {
        if(imageUrl is null) return new(false) { Data = "avatar.png" };
        var isSucceed = false;
        var imageName = Guid.NewGuid().ToString("N") + ".jpg";
        using (HttpClient webClient = new HttpClient()) 
        {
            byte [] data = await webClient.GetByteArrayAsync(imageUrl);

            using (MemoryStream mem = new MemoryStream(data)) 
            {
                await System.IO.File.WriteAllBytesAsync(Path.Combine(new string[5]{ "wwwroot", "Media", "User", "Images", imageName }), mem.ToArray());
                isSucceed = true;
            } 
        }
        if(isSucceed)
            return new(true) { Data = imageName };
        else
            return new(false) { Data = "avatar.png"};
    }

    public async ValueTask<bool> IsTelegramUserIdExistsAsync(ulong telegramUserId)
    => await _userManager.Users.AnyAsync(u => u.TelegramUserId == telegramUserId);

    public async ValueTask<Result> CreateUserAsync(ulong id, string? firstName, string? lastName, string? userName, string? userImageUrl)
    {
        if(id == default) return new("Telegram user id is invalid");

        var savedUserImageResult = await SaveUserImageAsync(userImageUrl);
        var user = new AppUser((firstName + lastName) ?? "anonymous", userName ?? "user"+id, string.Empty)
        {
            TwoFactorEnabled = true,
            Roles = new string[]
            {
                "user"
            },
            TelegramUserId = id,
            ImageUrl = savedUserImageResult.Data
        };
        try
        {
            var createUserResult = await _userManager.CreateAsync(user);
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

    public async ValueTask<Result<AppUser>> GetUserByTelegramIdAsync(ulong id)
    {
        if(id == default) return new("Telegram user id is invalid.");
        try
        {
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.TelegramUserId == id);
            if(existingUser is null) return new("The user with given Telegram User ID not found");

            return new(true) { Data = existingUser};
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(UserManagementService)}", e);
            throw new("Couldn't retrieve the user. Contact support", e);
        }
    }

    public bool IsAuthorizedInTelegram(ulong id, string? first_name, string? last_name, string? username, string? photo_url, ulong auth_date, string? checkHash, string key)
    {
        Dictionary<string, string?> auth_data = new Dictionary<string, string?>()
        {
            {"id", $"{id}"},
            {"first_name", first_name},
            {"last_name", last_name},
            {"username", username},
            {"photo_url", photo_url},
            {"auth_date", $"{auth_date}"}
        };

        List<string?> rawData = new List<string?>();
        
        foreach(var pair in auth_data)
        {
            if(pair.Value is null) continue;
            rawData.Add($"{pair.Key}={pair.Value}");
        }
        rawData.Sort();

        string hashData = string.Empty;
        
        for(int i = 0; i < rawData.Count; i++)
            hashData += $"{rawData[i]}{(i != rawData.Count-1 ? " " : "")}";
        
        var result = GenerateHashFromRawDataAndKey(hashData,key);
        if(result != checkHash)
            return false;
        else
            return true;
    }
    private string GenerateHashFromRawDataAndKey(string rawData, string key)
    {
        rawData = rawData.Replace(" ","\n");
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(key));
            var dataBytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashmessage = new HMACSHA256(bytes).ComputeHash(dataBytes);
            string sbinary = "";
            for (int i = 0; i < hashmessage.Length; i++)
                sbinary += hashmessage[i].ToString("X2");
            return sbinary.ToLower();
        }
    }
}