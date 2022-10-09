using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Services;

public interface IEmailSender
{
    ValueTask<Result> SendAsync(string to, string subject, string content);
}