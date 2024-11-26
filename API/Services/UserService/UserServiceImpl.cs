using API.Dtos.Response;
using API.Enums;
using API.Exceptions;
using API.Mappers;
using API.Models;
using API.Repositories;
using API.Services.Email;
using API.Services.UserService;

namespace API.Services.UserService;

public class UserServiceImpl : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IEmailService _emailService;
    public UserServiceImpl(IUserRepository userRepository, IPartnerRepository partnerRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _partnerRepository = partnerRepository;
        _emailService = emailService;
    }

    public async Task<GenericResponse> CreateUser(Users user, bool isPartner)
    {
        try
        {
            var emailCheck = await _userRepository.UserByEmail(user.Email);
            if (emailCheck != null)
            {
                return GenericResponse.FromError(new ErrorResponse("Duplicate entry", "Email exist already and in use ",
                    StatusCodes.Status400BadRequest),StatusCodes.Status400BadRequest);
            }
            
            
            var userObject = await  _userRepository.CreateUser(user);
            Console.WriteLine($"User Created Successfully: {userObject.Id}");
            if (isPartner)
            {
                Partner partner = new Partner
                {
                    UserId = userObject.Id,
                    BusinessNumber = user.Partner!.BusinessNumber,
                    Logo = user.Partner!.Logo,
                    Address = user.Partner!.Address,
                };
                await  _partnerRepository.CreatePartner(partner);
            }
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
        if (user.VerificationCode.Equals(verificationCode))
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
                StatusCodes.Status404NotFound),StatusCodes.Status404NotFound);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return GenericResponse.FromError(new ErrorResponse("An unexpected error occurred", e.Message, StatusCodes.Status500InternalServerError),StatusCodes.Status500InternalServerError);
        }
    }
    
    
    
    
    
    
    
}