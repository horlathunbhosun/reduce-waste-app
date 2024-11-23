using API.Dtos.User;

using API.Models;

namespace API.Mappers;

public static class UserMapper
{
        public static UserResponseDto ToUserResponseDto(this User userModel)
        {
            return new UserResponseDto
            {
                Id = userModel.Id,
                FullName = userModel.FullName,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber,
                UserType = userModel.UserType,
                IsVerified = userModel.IsVerified,
                Status = userModel.Status,
                Partner = userModel.Partner,
                CreatedAt = userModel.CreatedAt,
                UpdatedAt = userModel.UpdatedAt
            };
        }


        public static User ToUserRequestDto(this UserRequestDto userRequest)
        {
            return new User
            {
                FullName = userRequest.FullName,
                Email = userRequest.Email,
                PhoneNumber = userRequest.PhoneNumber,
                Password = userRequest.Password,
                UserType = userRequest.UserType,
                Partner = userRequest.Partner,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

    
}