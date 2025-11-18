using Application.DTOs.Profile;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IProfileService
{
    Task<UserProfileResponse> GetUser(int userId);
    Task UpdateProfile(int userId, UpdateProfileDto dto);
}