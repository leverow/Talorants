using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Talorants.Blog.Mvc.Models;

namespace Talorants.Blog.Mvc.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly HttpClient _httpClient;

    public EmailSender(ILogger<EmailSender> logger,HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    public async ValueTask<Result> SendAsync(string to, string subject, string content)
    {
        if(string.IsNullOrWhiteSpace(to) || !new EmailAddressAttribute().IsValid(to))
            return new("Destination email address is invalid.");

        if(string.IsNullOrWhiteSpace(subject))
            return new("Email subject cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(content))
            return new("Email content cannot be empty.");
        
        var requestEmail = GetEmailRequest(to, subject, content);

        var httpContent = new StringContent(
            JsonConvert.SerializeObject(requestEmail),
            Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/api/Email", httpContent);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(response.Content.ToString());
            return new(true);
        }
        
        _logger.LogWarning($"Email sending failed: {response.ReasonPhrase}");

        return new("Sending email failed. Contact support.");
    }

    private static EmailRequest GetEmailRequest(string to, string subject, string content)
    => new() 
    { 
        To = to, 
        Subject = subject, 
        Content = content, 
    };
}