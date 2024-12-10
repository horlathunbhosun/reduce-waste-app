using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    
    
    
}