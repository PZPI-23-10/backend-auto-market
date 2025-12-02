using Application.DTOs.Profile;

namespace Application.Interfaces.Services;

public interface IProfileService
{
    Task<UserProfileResponse> GetUser(int userId);
    Task<IEnumerable<UserProfileResponse>> GetAllUsers();
    Task UpdateProfile(int userId, UpdateProfileDto dto);
    Task DeleteUser(int userId);
    Task AddToRole(int userId, string role);
}