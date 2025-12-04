using Application.DTOs;
using Application.DTOs.Profile;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class ProfileService(
    IUserRepository users,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork,
    IFileUploadStrategyFactory uploadStrategyFactory,
    IFileHashService hashService,
    UserManager<User> userManager) : IProfileService
{
    public async Task<UserProfileResponse> GetUser(int userId)
    {
        User? user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new ApplicationException("User not found");

        IEnumerable<string> userRoles = await userManager.GetRolesAsync(user);
        
        return new UserProfileResponse
        {
            Id = user.Id,
            DateCreated = user.Created,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Country = user.Country,
            AboutUrself = user.AboutYourself,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            IsVerified = user.EmailConfirmed,
            IsGoogleAuth = user.IsGoogleAuth,
            AvatarUrl = user.Avatar?.Url,
            Roles = userRoles,
        };
    }

    public async Task<IEnumerable<UserProfileResponse>> GetAllUsers()
    {
        IEnumerable<User> allUsers = await users.GetAllAsync();

        var result = new List<UserProfileResponse>();

        foreach (var user in allUsers)
        {
            IList<string> roles = await userManager.GetRolesAsync(user);
            result.Add(new UserProfileResponse
            {
                Id = user.Id,
                DateCreated = user.Created,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                AboutUrself = user.AboutYourself,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                IsVerified = user.EmailConfirmed,
                IsGoogleAuth = user.IsGoogleAuth,
                AvatarUrl = user.Avatar?.Url,
                Roles = roles
            });
        }

        return result;
    }

    public async Task UpdateProfile(int userId, UpdateProfileDto dto)
    {
        var user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Country = dto.Country ?? user.Country;
        user.AboutYourself = dto.AboutYourself ?? user.AboutYourself;
        user.Address = dto.Address ?? user.Address;
        user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
        user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

        if (dto.Photo != null)
        {
            string avatarHash = hashService.ComputeHash(dto.Photo.Stream);

            if (user.Avatar is { IsExternal: false } && user.Avatar.Hash == avatarHash)
            {
                await unitOfWork.SaveChangesAsync();
                return;
            }

            IFileUploadStrategy uploadStrategy = uploadStrategyFactory.CreateFileUploadStrategy(PhotoCategory.Avatar);

            FileUploadResult uploadResult =
                await fileStorage.Upload(uploadStrategy, dto.Photo.Stream, dto.Photo.Name, userId);

            if (user.Avatar is { PublicId: not null })
            {
                await fileStorage.Delete(user.Avatar.PublicId);
            }

            user.Avatar ??= new UserAvatar();

            user.Avatar.Url = uploadResult.Url;
            user.Avatar.PublicId = uploadResult.PublicId;
            user.Avatar.Hash = avatarHash;
            user.Avatar.IsExternal = false;
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteUser(int userId)
    {
        var user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        users.Remove(user);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddToRole(int userId, string role)
    {
        if (role != UserRoles.Admin && role != UserRoles.User)
            throw new NotFoundException("Unknown role");

        User? user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        await userManager.AddToRoleAsync(user, role);
    }
}