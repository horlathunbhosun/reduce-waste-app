using API.Dtos.Response;

namespace API.Utilities;

public static class Constants
{
    public static class EmailClamValue{
        public const string Value = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    }
    
    
    public static class ErrorValidation
    {
        public static GenericResponse HanldeError(string message, string error, int statusCode)
        {
            return   GenericResponse.FromError(
                new ErrorResponse(message, error,
                    statusCode), statusCode);
        }
        
    }
}