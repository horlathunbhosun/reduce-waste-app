namespace API.Dtos.Response;

public class GenericResponse
{
    public ErrorResponse? Error { get; set; }
    
    public SuccessResponse? Success { get; set; }
    
    public int StatusCode { get; set; }
    
    private GenericResponse() { }

    public static GenericResponse FromError(ErrorResponse error, int statusCode)
    {
        return new GenericResponse { Error = error, StatusCode = statusCode };
    }
   

    public static GenericResponse FromSuccess(SuccessResponse success, int statusCode)
    {
        return new GenericResponse { Success = success, StatusCode = statusCode };
    }
    
}