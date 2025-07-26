using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace locktopus_domain.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    private readonly SendGridClient _client;
    
    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
        var apiKey = _config["SendGrid:ApiKey"];
        _client = new SendGridClient(apiKey);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var fromEmail = _config["SendGrid:SenderEmail"];
        var fromName  = _config["SendGrid:SenderName"];
        var from = new EmailAddress(fromEmail, fromName);
        var to   = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlMessage);
        var response = await _client.SendEmailAsync(msg).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation("SendGrid queued email to {Email}", toEmail);
        else
            _logger.LogError("SendGrid failed to {Email}: {Status}", toEmail, response.StatusCode);
    }
}