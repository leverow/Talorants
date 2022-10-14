using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Controllers;

public partial class AccountController
{
    public IActionResult Login(string? returnUrl)
    => View(new LoginViewModel() {ReturnUrl = returnUrl});

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid) return View(model);
        var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

        if(!signInResult.Succeeded) return View(model);

        return LocalRedirect($"{model.ReturnUrl ?? "/"}");
    }

    public async Task<IActionResult> LoginViaTelegramAsync([FromQuery]ulong id, [FromQuery]string? first_name, [FromQuery]string? last_name, [FromQuery]string? username, [FromQuery]string? photo_url, [FromQuery]ulong auth_date, [FromQuery]string? hash)
    {
        _logger.LogInformation($"{id},{first_name},{photo_url},{auth_date}");
        var existingUser = await _userManagement.GetUserByTelegramIdAsync(id);
        
        if(!_userManagement.IsAuthorizedInTelegram(id, first_name, last_name, username, photo_url, auth_date, hash, "5725742806:AAGwOJJgpbRddUCb-sjWscIhRv3LvdIexc0"))
            return Ok("Data is not from Telegram.");
        else
            _logger.LogInformation("✅ Yangi foydalanuvchi rostdan ham telegramdan keldi");

        if(!existingUser.IsSuccess)
        {
            var createUserResult = await _userManagement.CreateUserAsync(id, first_name, last_name, username, photo_url);
            _logger.LogInformation($"New telegram user {first_name} was created");
            ModelState.AddModelError(string.Empty, createUserResult.ErrorMessage ?? string.Empty);
        }
        else
        {
            await _signInManager.SignInAsync(existingUser.Data,false);
        }
        return LocalRedirect("/");
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}