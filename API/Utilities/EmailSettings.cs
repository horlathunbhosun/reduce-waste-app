namespace API.Utilities;

public class EmailSettings
{
    public string SMTPServer { get; set; } = string.Empty;
    public bool DefaultCredentials { get; set; }
    public int Port { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EmailId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSSL { get; set; }
}
