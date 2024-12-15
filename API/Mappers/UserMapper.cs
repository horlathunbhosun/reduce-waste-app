using API.Dtos.User;

using API.Models;
using API.Utilities;

namespace API.Mappers;

public static class UserMapper
{
        public static UserResponseDto ToUserResponseDto(this Users userModel)
        {
            return new UserResponseDto
            {
                Id = userModel.Id,
                FullName = userModel.FullName,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber,
                UserType = userModel.UserType,
                IsVerified = userModel.IsVerified,
                Partner = userModel.Partner?.ToPartnerResponseDto(),
                CreatedAt = userModel.CreatedAt,
                UpdatedAt = userModel.UpdatedAt
            };
        }
        
        
        


        public static Users ToUserRequestDto(this UserRequestDto userRequest)
        {
            return new Users
            {
                FullName = userRequest.FullName,
                Email = userRequest.Email,
                PhoneNumber = userRequest.PhoneNumber,
                UserName = userRequest.UserName,
                UserType = userRequest.UserType,
                VerificationCode = GenerateCode.GenerateRandomCode(6),
                Partner = userRequest.Partner?.ToPartnerDto(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
        }
        
        
        public static Partner ToPartnerDto(this PartnerDto partnerDto)
        {
            return new Partner
            {
                
                BusinessNumber = partnerDto.BusinessNumber,
                Logo = partnerDto.Logo,
                Address = partnerDto.Address,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
        
        
        public static PartnerResponseDto ToPartnerResponseDto(this Partner partner)
        {
            return new PartnerResponseDto
            {
                Id = partner.Uuid,
                BusinessNumber = partner.BusinessNumber,
                Logo = partner.Logo,
                Address = partner.Address,
                CreatedAt = partner.CreatedAt,
                UpdatedAt = partner.UpdatedAt
            };
        }

    
}