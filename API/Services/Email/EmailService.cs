using API.Utilities;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace API.Services.Email;




public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    
    private string LoadEmailTemplate(string templatePath, Dictionary<string, string> replacements)
    {
        var template = File.ReadAllText(templatePath);
        foreach (var placeholder in replacements)
        {
            template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }
        return template;
    }

    
    public async Task SendEmailWithTemplateAsync(string toEmail, string subject,  string templatePath, Dictionary<string, string> replacements)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.Name, _emailSettings.EmailId));
        email.To.Add(new MailboxAddress(toEmail, toEmail));
        email.Subject = subject;
        
        var emailBody = LoadEmailTemplate(templatePath, replacements);

        email.Body = new TextPart("html") { Text = emailBody };
        
        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_emailSettings.SMTPServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
            await smtp.SendAsync(email);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}