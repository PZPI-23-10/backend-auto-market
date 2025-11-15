using Application.DTOs.Profile;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IProfileService
{
    Task<User> GetUser(int userId);
    Task UpdateProfile(int userId, UpdateProfileDto dto);
}