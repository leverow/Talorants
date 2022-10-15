using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Services;

public class SettingsHandler
{
    public TelegramAuthorization Telegram;

    public SettingsHandler(IConfiguration configuration)
    {
        Telegram = configuration.GetSection("TelegramAuthorization").Get<TelegramAuthorization>();
    }
}