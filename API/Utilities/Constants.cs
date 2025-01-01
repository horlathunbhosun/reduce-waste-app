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
    
    


    public static string GetEmail(HttpContext httpContext)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == Constants.EmailClamValue.Value)?.Value;
        if (email == null)
        {
            return string.Empty;
        }

        return email;
    }


    public static bool CheckRole(HttpContext httpContext)
    {
        var role = httpContext.User.Claims.FirstOrDefault(c => c.Type == "userRole")?.Value;
        return role == "Partner";
    }


    public static string GetUserId(HttpContext httpContext)
    {
        var currentUser = httpContext.User;
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
        if (userId == null)
        {
            return string.Empty;
        }

        return userId;
    }
}