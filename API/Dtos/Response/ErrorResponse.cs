namespace API.Dtos.Response;

public class ErrorResponse
{
    public string Message { get; set; }
    public string Details { get; set; }
    
    public int StatusCode { get; set; }

    public ErrorResponse(string message, string details, int statusCode)
    {
        Message = message;
        Details = details;
        StatusCode = statusCode;
    }
}