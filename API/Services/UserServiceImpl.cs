using API.Models;
using API.Repositories;

namespace API.Services;

public class UserServiceImpl : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPartnerRepository _partnerRepository;
    public UserServiceImpl(IUserRepository userRepository, IPartnerRepository partnerRepository)
    {
        _userRepository = userRepository;
        _partnerRepository = partnerRepository;
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
       return userObject;
    }
}