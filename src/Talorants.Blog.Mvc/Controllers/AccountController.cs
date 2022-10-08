using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talorants.Blog.Mvc.Entities;

namespace Talorants.Blog.Mvc.Controllers;

public partial class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(
        ILogger<AccountController> logger,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }
}