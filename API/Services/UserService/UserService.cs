using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Dtos.Response;
using API.Dtos.User;
using API.Enums;
using API.Exceptions;
using API.Mappers;
using API.Models;
using API.Repositories;
using API.Repositories.Interface;
using API.Services.Email;
using API.Services.Token;
using API.Services.UserService;
using API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IEmailService _emailService;
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> _signinManager;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, IPartnerRepository partnerRepository, IEmailService emailService, UserManager<Users> userManager, SignInManager<Users> signinManager,ITokenService tokenService)
    {
        _userRepository = userRepository;
        _partnerRepository = partnerRepository;
        _emailService = emailService;
        _userManager = userManager;
        _signinManager = signinManager;
        _tokenService = tokenService;
    }

    public async Task<GenericResponse> CreateUser(Users user, bool isPartner, string password)
    {
        try
        {
            if (user.Email != null)
            {
                var emailCheck = await _userRepository.FindUserByEmail(user.Email);
                if (emailCheck != null)
                {
                    return GenericResponse.FromError(new ErrorResponse("Duplicate entry", "Email exist already and in use ",
                        StatusCodes.Status400BadRequest),StatusCodes.Status400BadRequest);
                }
            }


            var userObject = await  _userRepository.CreateUser(user,password);
            Console.WriteLine($"User Created Successfully: {userObject.Id}");

            await Task.Delay(2000);

           
            // Prepare replacements for the template
            if (userObject.VerificationCode != null)
            {
                var replacements = new Dictionary<string, string>
                {
                    { "Name", userObject.FullName },
                    { "VerificationCode", userObject.VerificationCode }
                };
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailTemplate.html");
                await _emailService.SendEmailWithTemplateAsync(user.Email, "Welcome to Our App", templatePath, replacements);

            }
       
            var roles = await _userManager.GetRolesAsync(userObject);
            var userResponseDto = userObject.ToUserResponseDto();
            userResponseDto.Roles = roles.ToList();


            if (isPartner)
            {

                var partner = new Partner
                {
                    UserId = userObject.Id,
                    BusinessNumber = user.Partner!.BusinessNumber,
                    Logo = user.Partner!.Logo,
                    Address = user.Partner!.Address,
                };
                await _partnerRepository.CreatePartner(partner);
            }
            var success = new SuccessResponse("User Created Successfully",
                userObject.ToUserResponseDto(), StatusCodes.Status200OK);
            return GenericResponse.FromSuccess(success, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return GenericResponse.FromError(new ErrorResponse("An unexpected error occurred", e.Message, StatusCodes.Status500InternalServerError),StatusCodes.Status500InternalServerError);

        }
       
    }


    public async Task<Users> GetUserByVerificationCode(string verificationCode)
     {
         var user = await _userRepository.FindUserByVerificationCode(verificationCode);
         if (user == null)
         {
             throw new NotFoundException($"Verification code {verificationCode} not valid");
         }
         if (user.VerificationCode != null && user.VerificationCode.Equals(verificationCode))
         {
             user.VerificationCode = "";
             user.Status = Status.Activated;
             user.IsVerified = true;
             var updatedUser = await _userRepository.UpdateUser(user);
             return updatedUser;
         }
 
         return user;
     }
     
     public async Task<GenericResponse> VerifyUserAsync(string verificationCode)
     {
         try
         {
             var user = await GetUserByVerificationCode(verificationCode);
             var success = new SuccessResponse("User Verified Successfully",
                 user.ToUserResponseDto(), StatusCodes.Status200OK);
             return GenericResponse.FromSuccess(success, StatusCodes.Status200OK);
         }
         catch (NotFoundException e)
         {
             return GenericResponse.FromError(new ErrorResponse("Verification code is invalid", e.Message,
                 StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
         }
         catch (Exception e)
         {
             Console.WriteLine(e);
             return GenericResponse.FromError(
                 new ErrorResponse("An unexpected error occurred", e.Message,
                     StatusCodes.Status500InternalServerError), StatusCodes.Status500InternalServerError);
         }

     
     }

     public async Task<GenericResponse> LoginUser(LoginRequestDto loginRequestDto)
     {
         var userCheck = await _userManager.Users.Include(u => u.Partner).FirstOrDefaultAsync(x => x.Email == loginRequestDto.Email!.ToLower());
         if (userCheck == null)
         {
             return GenericResponse.FromError(new ErrorResponse("Invalid Email address ", "Invalid Email Address",
                 StatusCodes.Status404NotFound), StatusCodes.Status404NotFound);
         }

         var checkPassword = await _signinManager.CheckPasswordSignInAsync(userCheck, loginRequestDto.Password!, false);
         if (!checkPassword.Succeeded)
         {
             return GenericResponse.FromError(new ErrorResponse("Invalid Email address and password ", "Invalid Email address and password",
                 StatusCodes.Status404NotFound), StatusCodes.Status404NotFound);
         }

         var checkUserVerified = userCheck is { IsVerified: true, Status: Status.Activated };
         if (checkUserVerified == false) 
         {
             return GenericResponse.FromError(new ErrorResponse("Account not verified ", "You need to be verified to login",
                 StatusCodes.Status404NotFound), StatusCodes.Status404NotFound); 
         }

         var createToken = _tokenService.CreateToken(userCheck);

         var jwtToken = new JwtToken
         {
             Token = createToken.Token,
             RefreshToken = createToken.RefreshToken,
             ExpiryTime = createToken.ExpiryTime,
             UserResponseDto = userCheck.ToUserResponseDto()
         };
         var success = new SuccessResponse("Login Successful", 
             jwtToken  , StatusCodes.Status200OK);
         return GenericResponse.FromSuccess(success, StatusCodes.Status200OK);
     }

     public async Task<GenericResponse> UpdateUser(UpdateUserRequestDto user, bool isPartner, string email)
     {
       
         try
         {
             var userDetails = await _userRepository.FindUserByEmail(email);
                if (userDetails == null)
                {
                    return GenericResponse.FromError(new ErrorResponse("User not found or logged in", "User not found or logged in",
                        StatusCodes.Status404NotFound), StatusCodes.Status404NotFound);
                }
                
                userDetails.Email = user.Email;
                userDetails.FullName = user.FullName;
                userDetails.PhoneNumber = user.PhoneNumber;
                userDetails.UserName = user.UserName;

                if (isPartner)
                {
                    
                        userDetails.Partner!.BusinessNumber = user.Partner!.BusinessNumber;
                        userDetails.Partner.Logo = user.Partner.Logo;
                        userDetails.Partner.Address = user.Partner.Address;
                    
                }
             
             var userUpdate = await _userRepository.UpdateUser(userDetails);
             var success = new SuccessResponse("User Updated Successfully",
                 userUpdate.ToUserResponseDto(), StatusCodes.Status200OK);
             return GenericResponse.FromSuccess(success, StatusCodes.Status200OK);

         }
         catch (Exception e)
         {
             return GenericResponse.FromError(new ErrorResponse("An unexpected error occurred", e.Message,
                 StatusCodes.Status500InternalServerError), StatusCodes.Status500InternalServerError);
         }
     }

     public async Task<GenericResponse> UserProfile(string email)
     {
         
         var userDetails = await _userRepository.FindUserByEmail(email);
         if (userDetails == null)
         {
             return GenericResponse.FromError(new ErrorResponse("User not found or logged in", "User not found or logged in",
                 StatusCodes.Status404NotFound), StatusCodes.Status404NotFound);
         }
         return GenericResponse.FromSuccess(new SuccessResponse("User Details", userDetails.ToUserResponseDto(), StatusCodes.Status200OK), StatusCodes.Status200OK);
     }

     public Partner? GetPartnerByUserId(string userId)
     {
         var partnersDetail =  _partnerRepository.FindPartnerByUserId(userId);


         return partnersDetail;

     }


     // public async Task<GenericResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
     // {
     //     var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
     //     if (principal == null)
     //     {
     //         throw new NotFoundException("Invalid refresh token");
     //     }
     //
     //     var user = await _userManager.FindByEmailAsync(principal.Identity.Name);
     //     if (user == null || user.RefreshToken != request.RefreshToken)
     //     {
     //         return BadRequest("Invalid refresh token");
     //     }
     //
     //     var newJwtToken = _tokenService.CreateToken(user);
     // }
     
     
    
}
    
    
    
    
    
    
    
