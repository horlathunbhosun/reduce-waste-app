namespace API.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject,  string body);
    Task SendEmailWithTemplateAsync(string toEmail, string subject,  string templatePath, Dictionary<string, string> replacements);

}