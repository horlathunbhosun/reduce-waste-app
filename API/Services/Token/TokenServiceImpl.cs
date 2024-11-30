using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Dtos.User;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace API.Services.Token;

public class TokenServiceImpl : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _symmetricSecurity; 
    // private readonly UserManager<IdentityUser> _userManager;
    // private readonly RoleManager<IdentityRole> _roleManager;

    public TokenServiceImpl(IConfiguration configuration)
    {
        _configuration = configuration;
        _symmetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
    }


    public JwtToken CreateToken(Users user)
    {
        // var userRoles =  _userManager.GetRolesAsync(user);
  
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
            new Claim("userRole", user.UserType)
        };
        

        var cred = new SigningCredentials(_symmetricSecurity, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = cred,
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"]
        };

        var tokenHanlder = new JwtSecurityTokenHandler();

        var token = tokenHanlder.CreateToken(tokenDescriptor);

        //return tokenHanlder.WriteToken(token);

        return new JwtToken
        {
            Token = tokenHanlder.WriteToken(token),
            ExpiryTime = tokenDescriptor.Expires.ToString(),
            RefreshToken = GenerateRefreshToken()
        };
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}