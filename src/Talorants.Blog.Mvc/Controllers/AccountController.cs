using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
}