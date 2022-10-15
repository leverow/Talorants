using Microsoft.AspNetCore.Identity;
using Talorants.Blog.Mvc.Entities;

public sealed class Seed
{
    public static async Task InitializeRolesAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Seed>();

        var roles = config.GetSection("Identity:IdentityServer:Roles").Get<string[]>();

        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new IdentityRole(role);
                var result = await roleManager.CreateAsync(newRole);

                if(!result.Succeeded)
                {
                    logger.LogWarning($"⚠️ Seed role {role} failed. Error: {result.Errors.First().Description}");
                }
                else
                {
                    logger.LogInformation("✅ Seed role {0} succeeded.", role);
                }
            }
        }
    }

    public static async Task InitializeUserAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Seed>();

        var users = config.GetSection("Identity:IdentityServer:DefaultUsers").Get<AppUser[]>();

        foreach(var user in users)
        {
            var newUser = new AppUser(user.Fullname ?? string.Empty, user.UserName, user.Email) { Roles = user.Roles };

            var result = await userManager.CreateAsync(newUser, user.PasswordHash);

            if(result.Succeeded)
            {
                var roleResult = await userManager.AddToRolesAsync(newUser, user.Roles);

                if(roleResult.Succeeded)
                {
                    logger.LogInformation($"Many roles have been added to {user.UserName}");
                }
                else
                {
                    logger.LogInformation($"Many roles haven't been added to {user.UserName}");
                }
            }
        }
    }
}