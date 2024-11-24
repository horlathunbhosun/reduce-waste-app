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

    public async Task<Users> CreateUser(Users user, bool isPartner)
    {
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
       
     

       return userObject;
    }
}