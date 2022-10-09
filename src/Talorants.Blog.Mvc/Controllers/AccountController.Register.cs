using Microsoft.AspNetCore.Mvc;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Controllers;

public partial class AccountController
{
    public IActionResult Register(string? returnUrl)
    => View(new RegisterViewModel() {ReturnUrl = returnUrl});

    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
    {
        if(!ModelState.IsValid)
            return View(model);
        
        var createUserResult = await _userManagement.CreateUserAsync(model.FullName,model.Username,model.Email,model.Password);
        _logger.LogInformation("New user was created");
        ModelState.AddModelError(string.Empty, createUserResult.ErrorMessage ?? string.Empty);
        return LocalRedirect($"/account/login?returnUrl={model.ReturnUrl}");
    }
}