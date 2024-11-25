namespace API.Dtos.Response;

public class SuccessResponse
{
    public string Message { get; set; }
    public Object Data { get; set; }
    
    public int StatusCode { get; set; }

    public SuccessResponse(string message, Object data, int statusCode)
    {
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }
}