using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Talorants.Blog.Mvc.Entities;
using Talorants.Blog.Mvc.Services;

namespace Talorants.Blog.Mvc.Controllers;

public partial class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserManagementService _userManagement;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(
        ILogger<AccountController> logger,
        IUserManagementService userManagement,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManagement = userManagement;
        _signInManager = signInManager;
    }
    
    [HttpGet("[controller]/userImage")]
    public async Task<FileContentResult> GetUserImage(string? name)
    {
        var retrievedUserPhotoResult = await _userManagement.GetUserPhotoByUserNameAsync(name);
        if(!retrievedUserPhotoResult.IsSuccess) return new FileContentResult(System.IO.File.ReadAllBytes(Path.Combine(new string[5]{ "wwwroot", "Media", "User", "Images", "avatar.png" })), "image/png");
        string contentType = "";
        new FileExtensionContentTypeProvider().TryGetContentType(retrievedUserPhotoResult.Data!, out contentType);
 
        string path = Path.Combine(new string[5]{ "wwwroot", "Media", "User", "Images", retrievedUserPhotoResult.Data! });
 
        byte[] bytes = System.IO.File.ReadAllBytes(path);
 
        return new FileContentResult(bytes, contentType);
    }
}