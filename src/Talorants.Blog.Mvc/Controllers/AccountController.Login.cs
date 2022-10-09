using Microsoft.AspNetCore.Mvc;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Controllers;

public partial class AccountController
{
    // public IActionResult Login(string? returnUrl)
    // => View(new LoginViewModel() {ReturnUrl = returnUrl});

    // [HttpPost]
    // public async Task<IActionResult> Login(LoginViewModel model)
    // {
    //     if(!ModelState.IsValid) return View(model);
    //     var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

    //     if(!signInResult.Succeeded) return View(model);

    //     return LocalRedirect($"{model.ReturnUrl ?? "/"}");
    // }

    // public async Task<IActionResult> Logout()
    // {
    //     await _signInManager.SignOutAsync();
    //     return RedirectToAction(nameof(Login));
    // }
}